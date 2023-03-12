# PoshFlickr
Do Flickr with Powershell and a DotNet Core Lib to boot.  Inspird by https://github.com/pwujczyk/ProductivityTools.PSFlickr

Also since it was needed a simple Flickr net core client hapily stealing from various other implementations like:
* https://github.com/arivoir/Open.Flickr
* https://github.com/samjudson/flickr-net/tree/v4/src (v4 branch that has core implementations)
* https://github.com/jamesyolk/flickr-api

My main goal is to be able to run this kind of commmand sequnce

```
Connect-Flickr

Get-Album AutoUpload | select -exp photos | Where {$_.Tags.length -gt 0} | Remove-Album AutoUpload
```

cause thats not possible in organzr