# ttab - TipToi Audio Book

ttab converts *.mp3 or *.ogg albums into a single *.gme file to play  on the [Ravensburger TipToi](https://www.ravensburger.de/shop/tiptoi/index.html) stick.

## Workflow
* Attach the TipToi stick to the USB port of your computer
* Run 
	ttab.exe [directory with audio files]
* Wait until ttab has transcoded the audio files
* A web page showing the contained albums pops up
* Print the web page
* Wait until ttab has copied the *.gme file to the stick

Internally, ttab uses following tools
* [mpg123-1.22.0-x86-64] to decode mp3 files
* [oggenc2.exe] to encode the ogg files for the TipToi stick
* [oggdec.exe] to decode input *.ogg files
* [tttool-win32-1.5.1](https://github.com/entropia/tip-toi-reveng) to aeemble the *.gme files