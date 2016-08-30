package
{
	import flash.display.MovieClip;
	import flash.display.Bitmap;
	import flash.events.Event;
	import flash.events.IOErrorEvent;
	import flash.events.ProgressEvent;
	import flash.net.URLRequest;
	import flash.display.Loader;
	import flash.external.ExternalInterface;
	import com.greensock.*;
	
	[SWF( backgroundColor="0xffff00" )]
	public class Main extends MovieClip 
	{
		// final coordinates of the image tween
		private var finalX:Number;
		private var finalY:Number;

		private var totalImages:Number;
		private var currentIndex:Number = 0;

		private var imageArray:Array;
		private var painterArray:Array;
		
		private var xml:XML;
		private var movieClip:MovieClip;
		
		public function Main() {
			ExternalInterface.addCallback("playSlideShow", playSlideShow);
			ExternalInterface.addCallback("stopSlideShow", stopSlideShow);
		}
		
		// called externally by the Windows UserControl 
		private function playSlideShow(xmlString:String) : void {
			movieClip = new MovieClip();
			movieClip.alpha = 0;    // hide the image until it is loaded

			xml = new XML(xmlString);
			var il:XMLList = xml.images;
			totalImages = il.length();
			
			populateArray();
		}
		
		// called externally by the Windows UserControl 
		private function stopSlideShow() : void {
			// stop and kill the movie clip tween
			stopAllMovieClips();
			removeChild(movieClip);
			TweenLite.killTweensOf(movieClip);
			imageArray = null;
			painterArray = null;
			movieClip = null;
		}
		
		private function populateArray():void {
			imageArray = new Array();
			painterArray = new Array();
			
			var i:Number;
			for (i = 0; i < totalImages; i++) {
				imageArray[i] = xml.images[i].pic;
			}
			beginImage();
		}
		
		private function beginImage():void {
			movieClip.scaleX = 1;
			movieClip.scaleY = 1;

			var imageLoader:Loader = new Loader();

			// catches errors if the loader cannot find the URL path
			imageLoader.contentLoaderInfo.addEventListener(IOErrorEvent.IO_ERROR, catchFunction);
			
			// actually loads the URL defined in the image array
			imageLoader.load(new URLRequest(imageArray[currentIndex]));
			
			// adds a listener for what to do when the image is done loading
			imageLoader.contentLoaderInfo.addEventListener(Event.COMPLETE, imgLoaded);

			function catchFunction(e:IOErrorEvent) : void {
				//trace("Bad URL: " + imageArray[currentIndex] + " does not exist");
				
				//take out the bad URL from the array
				imageArray.splice(currentIndex, 1);
				painterArray.splice(currentIndex, 1);

				if (imageArray.length == 0) {
					ExternalInterface.call("SlideShowComplete");
				} else {
					beginImage();
				}
			}

			function imgLoaded(event:Event):void {
				// add the image and get the dimensions to center the image
				movieClip.addChild(imageLoader);
				addChild(movieClip);
				
				// take the contents of the loaded image and cast it as bitmap data to allow for bitmap smoothing
				var image:Bitmap = imageLoader.content as Bitmap;
				image.smoothing = true;
				movieClip.x = (stage.stageWidth / 2) - (imageLoader.content.width / 2);
				movieClip.y = (stage.stageHeight / 2) - (imageLoader.content.height / 2);

				finalX = (stage.stageWidth / 2) - (imageLoader.content.width * .9 / 2);
				finalY = (stage.stageHeight / 2) - (imageLoader.content.height * .9 / 2);

				//start tween function
				easeIn();
			}
		}

		private function easeIn():void {
			TweenLite.to(movieClip, 6, {scaleX:.9, scaleY:.9, x:finalX, y:finalY, onComplete:hideStuff});
			TweenLite.to(movieClip, 1, {alpha:1, overwrite:0});
		}

		private function hideStuff():void {
			TweenLite.to(movieClip, 1, {alpha:0, onComplete:nextImage});
		}

		private function nextImage():void {
			// take out the image that was just displayed
			imageArray.splice(currentIndex, 1);
			painterArray.splice(currentIndex, 1);

			// remove the picture
			movieClip.removeChildAt(0);

			if (imageArray.length == 0) {
				// raise the SlideShowComplete event
				ExternalInterface.call("SlideShowComplete");
			} else {
				beginImage();
			}
		}
	}
}