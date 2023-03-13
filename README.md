# PoshFlickr
Do Flickr with Powershell and a DotNet Core Lib to boot.  Inspird by https://github.com/pwujczyk/ProductivityTools.PSFlickr

Also since it was needed a simple Flickr net core client hapily stealing from various other implementations like:
* https://github.com/arivoir/Open.Flickr
* https://github.com/samjudson/flickr-net/tree/v4/src (v4 branch that has core implementations)
* https://github.com/jamesyolk/flickr-api

My main goal is to be able to run this kind of commmand sequnce

```
Connect-Flickr

Get-FlickrAlbum AutoUpload | select -exp photos | Where {$_.Tags.length -gt 0} | Remove-Album AutoUpload
```

cause thats not possible in organzr

this targets .net6 because i was to lazy to figure out how to make record types target 2.1, and i don't really care to have that kind of compatability anyways


Known best cmdlet practices NOT followed:
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/requesting-confirmation-from-cmdlets?view=powershell-7.3 - arent confirming anything
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/credential-attribute-declaration?view=powershell-7.3 not using credential type correctly
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/strongly-encouraged-development-guidelines?view=powershell-7.3#support-the-passthru-parameter - support pass thru param
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/strongly-encouraged-development-guidelines?view=powershell-7.3#create-a-cmdlet-help-file-sd05 - need to do help
* https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/defining-default-member-sets-for-objects?view=powershell-7.3 - wrapping flickr types with default display stuff and allowing extra via -Property / -prop