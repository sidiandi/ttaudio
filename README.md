# ttaudio - TipToi Audio

ttaudio converts a collection of *.mp3 or *.ogg audio files into a single *.gme file to play on the [Ravensburger TipToi](https://www.ravensburger.de/shop/tiptoi/index.html) pen.

![Screenshot of MainForm](/doc/screenshot_mainform.png)

The main window

![Screenshot of HTML output](/doc/screenshot_html_output.png)

HTML output

[Here is an example of the HTML page.](/doc/example.pdf)

## Workflow
* Attach the TipToi pen to the USB port of your computer
* Start ttaudio
* Drop audio files into the file list
* Click "Convert and Copy to Pen"
* The conversion of audio files starts
* A web page with optical IDs to play the audio files opens
* Print the web page
* Wait until ttaudio has copied the *.gme file to the pen

## More possibilities

ttaudio is based on [tttool](http://tttool.entropia.de/), a universal, command line based tool to create and analyze GME files. 
Check out http://tttool.entropia.de/ if you want to do more than just upload some MP3 files, e.g. print your own interactive books or toys.

## Dependencies

Internally, ttaudio uses following tools:
* [mpg123-1.22.0-x86-64](http://www.mpg123.de/download.shtml) to decode mp3 files
* [oggenc2.exe](http://www.rarewares.org/ogg-oggenc.php) to encode the ogg files for the audio pen
* [oggdec.exe](http://www.rarewares.org/ogg-oggdec.php) to decode input *.ogg files
* [tttool-win32-1.5.1](https://github.com/entropia/tip-toi-reveng) to assemble the *.gme files

