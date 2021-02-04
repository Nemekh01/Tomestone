# Tomestone

Web API server implementation of Lumina with AWS Docker based off of Adam's demo.

Some quick things:
* Requires the `ffxiv/0a*` files to be located in the datapath you provide
* Some things break, due to cyclic references
* The initial load of a sheet will be _slowish_ but subsequent loads will be very fast (< 1ms) as the data will be cached in memory from thereon
* Lumina is single threaded, normally this isn't a problem but you will need to warm up the row cache for each sheet if you want to avoid this problem when the api is under high load


# Usage
Hit `<api host>/sheets/{language}/{sheetName}/{rowId}[/{subRowId}}` and you get data back.

`subRowId` is optional and not required, only provide if you require it.

## Examples

### Actions

https://tomestone.akhmorning.com/sheets/english/action/23290

### Statuses

https://tomestone.akhmorning.com/sheets/english/status/2470

### Items

https://tomestone.akhmorning.com/sheets/english/item/29725

# Languages

Languages can be any of the following:

|Language|Language ID|
|:--|:--:|
|`None`|0|
|`Japanese`|1|
|`English`|2|
|`German`|3|
|`French`|4|
|`ChineseSimplified`|5|
|`ChineseTraditional`|6|
|`Korean`|7|

Note that Korean and Chinese support requires the separate game files that come with those languages.

---

**Final Fantasy XIV © 2010-2021 SQUARE ENIX CO., LTD. All Rights Reserved. We are not affiliated with SQUARE ENIX CO., LTD. in any way.**
