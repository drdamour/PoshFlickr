# PoshFlickr
Do Flickr with Powershell and a DotNet Core Lib to boot.  Inspird by https://github.com/pwujczyk/ProductivityTools.PSFlickr

Also since it was needed a simple Flickr net core client hapily stealing from various other implementations like:
* https://github.com/arivoir/Open.Flickr
* https://github.com/samjudson/flickr-net/tree/v4/src (v4 branch that has core implementations)
* https://github.com/jamesyolk/flickr-api

My main goal was to be able to run this kind of commmand sequence

```
Connect-Flickr [apikey] [apisecret]

Get-FlickrAlbum -Name "Auto Upload" | Get-FlickrAlbumPhotos -prop tags | Where {$_.Tags.length -gt 0 -and $_.Public} | Remove-FlickrAlbumPhoto
```

cause removing from albums is not possible in flickr's organzr.  That was achieved mar 17 2023 and unless i get interest i dont plan to domuch more than that.

this targets .net6 because i was to lazy to figure out how to make record types target 2.1, and i don't really care to have that kind of compatability anyways, everyone should be on dotnet 6+ soon enough


Known best cmdlet practices NOT followed:
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/requesting-confirmation-from-cmdlets?view=powershell-7.3 - arent confirming anything
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/credential-attribute-declaration?view=powershell-7.3 not using credential type correctly
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/strongly-encouraged-development-guidelines?view=powershell-7.3#support-the-passthru-parameter - support pass thru param
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/strongly-encouraged-development-guidelines?view=powershell-7.3#create-a-cmdlet-help-file-sd05 - need to do help
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/defining-default-member-sets-for-objects?view=powershell-7.3 - wrapping flickr types with default display stuff and allowing extra via -Property / -prop