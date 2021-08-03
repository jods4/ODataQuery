# ODataQuery
This package enables server-side filtering, sorting and pagination of any `IQueryable<T>` using OData syntax and without needing an EDM model.
This is achieved simply by adding `[ODataQueryable]` to any such API.

## How to use
Add a reference to Nuget package `ODataQuery`.

```csharp
// Import this namespace
using ODataQuery;

[Route("api/version")]
public class VersionController
{
  // Add attribute ODataQueryable
  [HttpGet, ODataQueryable]
  public IQueryable<Version> Get()
  {
    // Source data can be anything (most likely an EF query)...
    var versions = new[] {
      new Version(1, 0),
      new Version(1, 1),
      new Version(2, 0),
      new Version(3, 0),
    };
    // ... as long as it's IQueryable<T>
    return versions.AsQueryable();
  }
}
```

Now you can filter, sort and paginate versions:
```
GET /api/version?$filter=major gt 2&$orderby=major desc
```
would return this response:
```json
{
  "value": [
    { "major": 3, "minor": 0 },
    { "major": 2, "minor": 0 }
  ]
}
```

`ODataQueryable` changes the shape of your response to a standard OData response:
- `{ "value": [...] }` by default;
- `{ "@odata.count": 42, "value": [...] }` if you used the `$count` option.

## Limitations
The goal of this project is to enable easy server-side processing of datagrids, lists and co. in web applications, while being secure by default
(e.g. you can't fetch the full DB through `$expand`, because it's not supported).

Currently it has the following limitations:
- There is no routing, only query string processing.
- Not all system options are supported, only: `$filter`, `$search`, `$orderby`, `$skip`, `$take`, `$select` and `$count`.
- Only a subset of the full OData 4.0.1 is supported, see below for detailed support.
- Error reporting is bad.

That last point warrants an explanation.
You will get a `ParseException` if the query string is not correctly formatted and the message will indicate where it failed,
but the error message can often be unhelpful: e.g. mentioning a single expected character instead of a full token.

You will also get various exceptions if the query can be parsed but is semantically incorrect (properties that don't exist, type errors, invalid date literals).

In all cases this will likely result in a _500 Internal Error_, although a 400 code (bad request) would be more appropriate.

With respect to error-friendliness, this API is not meant to be used to build public, open APIs.

## Usage

### ODataQueryable attribute
Apply `[ODataQueryable]` to automatically apply OData query string to an `IQueryable<T>` result and return an OData response.

If the method throws, doesn't return a success code (2xx) or doesn't return an `IQueryable<T>` result then this attribute does nothing.

### Manual application
This package adds an extension method to `IQueryable<T>` to apply the query string manually in your code:
```csharp
IQueryable<T> source;
// Does not apply $count
IQueryable<T> result = source.OData(HttpContext.Request.Query);
// Applies $count and return result in out parameter
int count;
IQueryable<T> result = source.OData(HttpContext.Request.Query, out count);
```

### Fine-grained
Two extension methods let you apply `$filter` or `$orderby` directly from a string:
```csharp
IQueryable<T> source;
IQueryable<T> result = source.ODataFilter("amount gt 1000")
                             .ODataOrderBy("release desc,id");
```

## Extensions to OData 4.0.1
This library follows OData conventions, with the following extensions:

- `DateTime` is supported (OData only supports `DateTimeOffset`).
Literals with a timezone parse as `DateTime`, literals with a timezone as `DateTimeOffset`.

- `in` operator can have an empty list on right hand side: `x in ()`.
This is forbidden by OData grammar but it is accepted by this library and evaluates to `false`.

## Supported grammar
This library implements a subset of OData 4.0.1.

Only `$filter`, `$orderby`, `$take`, `$skip` and `$count` are supported.

### Types and Literals
Supported: Numeric types, `DateTime` and `DateTimeOffset`, `bool`, `string`, enums.

Literals:
- strings delimited with single quotes, double single-quote escape: `'Marc''s house'`
- numbers, exponents are **not** supported: `-? [0-9]+ (. [0-9]+)?`
- dates, day then optional time then optional timezone (Z or +-0000), e.g. `2019-07-22`, `2019-07-22T14:15:00`, and `2019-07-22T14:15:00+0200`
- enums, either as string or ints. Old syntax prefixed by type name is **not** supported: `'red'`.
- `true`, `false`
- `null`

### Identifiers
Starting with a letter or `_`, then followed by letters, digits and `_`.

Identifier are looked for in a case-insensitive way in model. So `id eq 3` will match a property `Id` on the server.

Path to subproperties such as `Address/City` are **not** supported.

### Operators
Comparison operators:
- Supported: `eq`, `ne`, `lt`, `le`, `gt`, `ge`, `in`
- **Not** supported: `has`

Logical operators:
- Supported: `and`, `or`, `not`

Arithmetic operators:
- **Not** supported: `add`, `sub`, `mul`, `div`, `divby`, `mod`.

Grouping operators:
- Supported: `()`

### Functions
String functions:
- Supported: `contains`, `endswith`, `startswith`, `length`, `indexof`, `substring`, `tolower`, `toupper`, `trim`, `concat`

Date functions:
- Supported: `year`, `month`, `day`, `hour`, `minute`, `second`, `date`.
- **Not** supported: `fractionalseconds`, `time`, `totaloffsetminutes`, `now`, `mindatetime`, `maxdatetime`.

Math functions:
- Supported: `round`, `floor`, `ceiling`.

Type functions:
- **Not** supported: `cast`, `isof`.

Geo function:
- **Not** supported: `geo.distance`, `geo.length`, `geo.intersects`.

## $filter
Filter must evaluate to any boolean expression and is applied as `Where`.

Collection predicates such as `any` and `all` are **not** supported, neither are paths to sub-properties such as `Address/City`.

## $search
The semantics of what search does is up to the application: which fields? case-sensitive? starts, contains or exact match?

For this reason, the attribute `ODataQueryable` simply binds a `$search` query string to a `search` parameter (if any), so that you can apply the search yourself.

## $orderby
Sorting is a comma-separated list of expressions, optionnally followed by keywords `asc` and `desc`.

It is applied as `OrderBy`, `ThenBy`, respectively `OrderByDescending`, `ThenByDescending`.

**Note:** OData grammar doesn't allow spaces before or after commas in `$orderby`, neither does this library. So `$orderby=name, id` will throw.

## $select
Projecting only supports a list of plain identifiers.

It is applied as a `Select` that returns a lightweight `IDictionary<string, object>` that is meant to be serialized (it is not fully functional and most methods throw `NotImplementedException`).

## $count
If `$count=true` is in the query string, the response will have an additional `@odata.count` property set to the count of results just after applying filter, i.e. _before_ applying `$take`, `$skip`, and `$orderby`.

It is applied as a call to `Count` (so 2 queries are executed).

**Note:** only the OData 4.0 `$count` option is supported. Previously it was `$inline-count=allpages`, which is **not** supported by this library.

## $take and $skip
Both supported as plain integers, applied as `Take` and `Skip` calls.