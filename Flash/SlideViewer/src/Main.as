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
			ExternalInterface.addCallback("initializeMovie", initializeMovie);
			ExternalInterface.addCallback("showImage", showImage);
			ExternalInterface.addCallback("hideImage", hideImage);
			ExternalInterface.addCallback("stopImage", stopImage);
		}
		
		// called externally by the Windows UserControl 
		private function initializeMovie() : void {
			movieClip = new MovieClip();
			movieClip.alpha = 0;    // hide the image until it is loaded
		}
		
		// called externally by the Windows UserControl 
		private function showImage(xmlString:String) : void {
			imageArray = new Array();
			painterArray = new Array();
			xml = new XML(xmlString);
			
			if (numChildren == 1) {
				stopAllMovieClips();
				movieClip.removeChildAt(0);
				removeChild(movieClip);
				TweenLite.killTweensOf(movieClip);
			}
			
			imageArray[0] = xml.images[0].pic;
			beginImage();
		}
		
		// called externally by the Windows UserControl 
		private function stopImage() : void {
			// stop and kill the movie clip tween
			stopAllMovieClips();
			removeChild(movieClip);
			TweenLite.killTweensOf(movieClip);
			imageArray = null;
			painterArray = null;
			movieClip = null;
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
			TweenLite.to(movieClip, 6, {scaleX:.9, scaleY:.9, x:finalX, y:finalY});
			TweenLite.to(movieClip, 1, {alpha:1, overwrite:0});
		}

		private function hideImage():void {
			TweenLite.to(movieClip, 1, {alpha:0, onComplete: imageHidden});
		}

		private function imageHidden():void {
			//movieClip.removeChildAt(0);
			ExternalInterface.call("FlashCall")
		}
	}
}