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
		// uncomment to test
		//private var xmlStringTest:String = "<xml><images><pic>\\\\Dev1\\sqlexpress\\KeebeeAATFilestream\\Media\\Profiles\\0\\images\\general\\2008_01_30.jpg</pic></images></xml>"; 
	
		private var clientWidth:Number = 1920;
		private var clientHeight:Number = 1080;
		
		// final coordinates of the image tween
		private var finalX:Number;
		private var finalY:Number;

		private var currentIndex:Number = 0;

		private var imageArray:Array;
		private var painterArray:Array;
		
		private var xml:XML;
		private var movieClip:MovieClip;
		
		public function Main() {
			// comment to test
			ExternalInterface.addCallback("initializeMovie", initializeMovie);
			ExternalInterface.addCallback("showImage", showImage);
			ExternalInterface.addCallback("hideImage", hideImage);
			ExternalInterface.addCallback("stopImage", stopImage);
			
			// uncomment to test
			//initializeMovie();
			//showImage(xmlStringTest);
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
			
			if (numChildren > 0) {
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
				setImageScale(imageLoader);
				
				// add the image and get the dimensions to center the image
				movieClip.addChild(imageLoader);
				addChild(movieClip);
				
				// take the contents of the loaded image and cast it as bitmap data to allow for bitmap smoothing
				var imgRatio:Number = imageLoader.content.width / imageLoader.content.height;
				var stageRatio:Number = stage.stageWidth / stage.stageHeight;
				
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

		private function setImageScale(imageLoader:Loader):void {
            var newWidth:Number;
            var newHeight:Number;
				
            var screenRatio:Number = clientWidth / clientHeight;
            var imageRatio:Number = imageLoader.content.width / imageLoader.content.height;

            // if the picture is landscape or a perfect square
            if (imageRatio >= 1)
            {
                // if the picture is "more square" than the screen
                if (imageRatio < screenRatio)
                {
                    newHeight = clientHeight;
                    newWidth = newHeight * imageRatio;
                }

                // if the picture is "less square" than the screen
                else
                {
                    newWidth = clientWidth;
                    newHeight = newWidth / imageRatio;
                }
            }

            // the picture is portrait
            else
            {
                newHeight = clientHeight;
                newWidth = newHeight * imageRatio;
            }

            imageLoader.content.width = newWidth;
			imageLoader.content.height = newHeight;
        }
		
		private function easeIn():void {
			TweenLite.to(movieClip, 6, {scaleX:.9, scaleY:.9, x:finalX, y:finalY});
			TweenLite.to(movieClip, 1, {alpha:1, overwrite:0});
		}

		private function hideImage():void {
			TweenLite.to(movieClip, 1, {alpha:0, onComplete: imageHidden});
		}

		private function imageHidden():void {
			// comment to test
			ExternalInterface.call("FlashCall")
		}
	}
}