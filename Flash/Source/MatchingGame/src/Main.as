package
{
	import flash.external.ExternalInterface;
	
	import com.greensock.*;
	import com.greensock.plugins.*;
	import com.greensock.easing.*;
	
	import flash.net.URLRequest;
	import flash.net.URLLoader;
	import flash.media.SoundMixer;
	import flash.media.Sound;
	import flash.display.Sprite;
	import flash.display.Shape;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.IOErrorEvent;
	import flash.display.Loader;
	import flash.display.Bitmap;
	import flash.utils.setTimeout;
	import flash.utils.clearTimeout;

	import flash.text.TextField;
	import flash.text.TextFormat;
	import flash.text.TextFormatAlign;
	import flash.text.AntiAliasType;
	
	import Assets.TextAssets;

	[SWF(backgroundColor="0xffff00")]
	public class Main extends Sprite 
	{		
		private var Picture:Assets.TextAssets;
		private var gameTimeout:uint;
		
		private var stageWidth:uint = 1920;
		private var stageHeight:uint = 1080;
		private var halfStageWidth:uint = stageWidth / 2;
		private var halfStageHeight:uint = stageHeight / 2;
		private var thirdStageWidth:uint = stageWidth / 3;
		private var thirdStageHeight:uint = stageHeight / 3;
		private var fourthStageWidth:uint = stageWidth / 4;
		private var fourthStageHeight:uint = stageHeight / 4;
		private var fifthStageWidth:uint = stageWidth / 5;
		private var fifthStageHeight:uint = stageHeight / 5;
		private var sixthStageWidth:uint = stageWidth / 6;
		private var sixthStageHeight:uint = stageHeight / 6;
		private var boardWidth:uint = 1;
		private var boardHeight:uint = 2;

		private var verticalLine:Shape;
		private var currentLevel:Number;
		private var timeoutValue:Number = 900000;
		
		// for standalone testing
		
		/*
		private const UserName:String = "AAT";  // <-- your windows username
		private const PathShapesRoot:String = "C:\\Users\\" + UserName + "\\Source\\Repos\\R2G2V1\\Media\\SharedLibrary\\activities\\matching-game\\shapes\\";
		private var xmlShapesTest:String = "<images><image><name>" + PathShapesRoot + "helicopter.png</name></image><image><name>" + PathShapesRoot + "phone.png</name></image><image><name>" + PathShapesRoot + "hammer-and-nail.png</name></image><image><name>" + PathShapesRoot + "fish.png</name></image><image><name>" + PathShapesRoot + "envelope.png</name></image><image><name>" + PathShapesRoot + "hand-drill.png</name></image><image><name>" + PathShapesRoot + "deer.png</name></image><image><name>" + PathShapesRoot + "crescent-moon.png</name></image><image><name>" + PathShapesRoot + "hot-air-ballon.png</name></image><image><name>" + PathShapesRoot + "camel.png</name></image><image><name>" + PathShapesRoot + "motorcycle.png</name></image><image><name>" + PathShapesRoot + "rhinoceros.png</name></image><image><name>" + PathShapesRoot + "cloud.png</name></image><image><name>" + PathShapesRoot + "dump-truck.png</name></image><image><name>" + PathShapesRoot + "moon.png</name></image><image><name>" + PathShapesRoot + "bear.png</name></image><image><name>" + PathShapesRoot + "atv.png</name></image><image><name>" + PathShapesRoot + "modern-train.png</name></image><image><name>" + PathShapesRoot + "elephant.png</name></image><image><name>" + PathShapesRoot + "track-loader.png</name></image><image><name>" + PathShapesRoot + "chicken.png</name></image><image><name>" + PathShapesRoot + "bird.png</name></image><image><name>" + PathShapesRoot + "maple-leaf.png</name></image><image><name>" + PathShapesRoot + "umbrella.png</name></image><image><name>" + PathShapesRoot + "butterfly.png</name></image><image><name>" + PathShapesRoot + "parrot.png</name></image><image><name>" + PathShapesRoot + "pickup-truck.png</name></image><image><name>" + PathShapesRoot + "panther.png</name></image><image><name>" + PathShapesRoot + "horse.png</name></image><image><name>" + PathShapesRoot + "duck.png</name></image><image><name>" + PathShapesRoot + "snail.png</name></image><image><name>" + PathShapesRoot + "dog.png</name></image><image><name>" + PathShapesRoot + "cement-mixer.png</name></image><image><name>" + PathShapesRoot + "delivery-truck.png</name></image><image><name>" + PathShapesRoot + "car-headlights.png</name></image><image><name>" + PathShapesRoot + "clover-leaf.png</name></image><image><name>" + PathShapesRoot + "house.png</name></image><image><name>" + PathShapesRoot + "bus.png</name></image><image><name>" + PathShapesRoot + "bicycle.png</name></image><image><name>" + PathShapesRoot + "airplane.png</name></image><image><name>" + PathShapesRoot + "road-roller.png</name></image><image><name>" + PathShapesRoot + "monkey.png</name></image><image><name>" + PathShapesRoot + "crane.png</name></image><image><name>" + PathShapesRoot + "light-bulb.png</name></image><image><name>" + PathShapesRoot + "pine-tree.png</name></image><image><name>" + PathShapesRoot + "hemp.png</name></image><image><name>" + PathShapesRoot + "sun.png</name></image><image><name>" + PathShapesRoot + "cat.png</name></image><image><name>" + PathShapesRoot + "rabbit.png</name></image><image><name>" + PathShapesRoot + "steam-train.png</name></image><image><name>" + PathShapesRoot + "jet-airplane.png</name></image><image><name>" + PathShapesRoot + "bucket-loader.png</name></image><image><name>" + PathShapesRoot + "flower.png</name></image><image><name>" + PathShapesRoot + "sail-boat.png</name></image><image><name>" + PathShapesRoot + "cow.png</name></image><image><name>" + PathShapesRoot + "kangaroo.png</name></image><image><name>" + PathShapesRoot + "refrigerated-truck.png</name></image><image><name>" + PathShapesRoot + "pig.png</name></image></images>";
		
		private const PathSoundsRoot:String = "C:\\Users\\" + UserName + "\\Source\\Repos\\R2G2V1\\Media\\SharedLibrary\\activities\\matching-game\\sounds\\";
		private var wouldYouLikeToMatchThePicturesTest:String = PathSoundsRoot + "would-you-like-to-match-the-pictures.mp3";
		private var wouldYouLikeToMatchThePairsTest:String = PathSoundsRoot + "would-you-like-to-match-the-pairs.mp3";
		private var correctTest:String = PathSoundsRoot + "correct.mp3";
		private var goodJobTest:String = PathSoundsRoot + "good-job.mp3";
		private var wellDoneTest:String = PathSoundsRoot + "well-done.mp3";
		private var TryAgainTest:String = PathSoundsRoot + "try-again.mp3";
		private var letsTryAgainTest:String = PathSoundsRoot + "lets-try-again.mp3";
		private var letsTrySomethingDifferentTest:String = PathSoundsRoot + "lets-try-something-different.mp3";		
		*/
		
		// media
		private var xmlShapes:String;
		private var pathMedia:String;
		
		// arrays
		private var images:Array;
		private var loadedImages:Array;
		private var mainImage:Array;
		
		// match-the-pairs arrays
		private var imagesPairs:Array;
		private var loadedImagesPairs:Array;
		private var mainImagePairs:Array;
		private var clickedImagesPairs:Array;
		private var childImagesPairs:Array;
		private var clickedImagesPairsInstance:Array;
				
		// images
		private var images_xml:XML;
		
		// sounds
		private var urlSoundCorrect:URLRequest;
		private var urlSoundGoodJob:URLRequest;
		private var urlSoundWellDone:URLRequest;
		private var urlSoundTryAgain:URLRequest;
		private var urlSoundLetsTryAgain:URLRequest;
		private var urlSoundLetsTrySomethingDifferent:URLRequest;
		private var urlSoundWouldYouLikeToMatchThePictures:URLRequest;
		private var urlSoundWouldYouLikeToMatchThePairs:URLRequest;
		
		private var soundCorrect:Sound;
		private var soundGoodJob:Sound;
		private var soundWellDone:Sound;
		private var soundTryAgain:Sound;
		private var soundLetsTryAgain:Sound;
		private var soundLetsTrySomethingDifferent:Sound;
		private var soundWouldYouLikeToMatchThePictures:Sound = new Sound();
		private var soundWouldYouLikeToMatchThePairs:Sound;

		private var clickCount:Number;
		private var numAttempts:Number;
		private var enableGameTimeout:Boolean;
		
		public function Main() {
			if (stage) init();
			else addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event = null):void {
			removeEventListener(Event.ADDED_TO_STAGE, init);
			Assets.TextAssets.init();

			verticalLine = new Shape();
			TweenPlugin.activate([TransformMatrixPlugin, ShortRotationPlugin]);
			
			// comment the following 3 lines to test
			ExternalInterface.addCallback("loadMedia", loadMedia);
			ExternalInterface.addCallback("playMatchingGame", playMatchingGame);
			ExternalInterface.addCallback("stopMatchingGame", stopMatchingGame);
			
			// uncomment to test
			/*
			loadMedia(xmlShapesTest, wouldYouLikeToMatchThePicturesTest, wouldYouLikeToMatchThePairsTest, 
				correctTest, goodJobTest, wellDoneTest, TryAgainTest, letsTryAgainTest, letsTrySomethingDifferentTest, 1, 1);
				
			playMatchingGame();
			*/
		}
		
		private function LogInteractiveActivityEvent(success:Number, description:String, isGameHasExpired:Boolean = false):void {
			var successDesc:String = "NULL";
			if (success == 0) successDesc = "FALSE";
			else if (success == 1) successDesc = "TRUE";
			
			// comment the following line to test
			ExternalInterface.call("FlashCall", currentLevel, successDesc, description, isGameHasExpired);
		}
		
		// called externally by the Windows UserControl
		private function loadMedia(xmlShapePaths:String, 
			wouldYouLikeToMatchThePicturesPath:String, 
			wouldYouLikeToMatchThePairsPath:String,
			correctPath:String,
			goodJobPath:String,
			wellDonePath:String,
			tryAgainPath:String,
			letsTryAgainPath:String,
			letsTrySomethingDifferentPath:String,
			difficultyLevel:Number, 
			enableTimeout:Number):void {

			xmlShapes = xmlShapePaths;
			enableGameTimeout = (enableTimeout == 1);
			
			urlSoundWouldYouLikeToMatchThePictures = new URLRequest(wouldYouLikeToMatchThePicturesPath);
			urlSoundWouldYouLikeToMatchThePairs = new URLRequest(wouldYouLikeToMatchThePairsPath);
			urlSoundCorrect = new URLRequest(correctPath);
			urlSoundGoodJob = new URLRequest(goodJobPath);
			urlSoundWellDone = new URLRequest(wellDonePath);
			urlSoundTryAgain = new URLRequest(tryAgainPath);
			urlSoundLetsTryAgain = new URLRequest(letsTryAgainPath);
			urlSoundLetsTrySomethingDifferent = new URLRequest(letsTrySomethingDifferentPath);
			
			soundCorrect = new Sound();
			soundGoodJob = new Sound();
			soundWellDone = new Sound();
			soundTryAgain = new Sound();
			soundLetsTryAgain = new Sound();
			soundLetsTrySomethingDifferent = new Sound();
			
			soundWouldYouLikeToMatchThePictures = new Sound();
			soundWouldYouLikeToMatchThePairs = new Sound();
			
			soundCorrect.load(urlSoundCorrect);
			soundGoodJob.load(urlSoundGoodJob);
			soundWellDone.load(urlSoundWellDone);
			soundTryAgain.load(urlSoundTryAgain);
			soundLetsTryAgain.load(urlSoundTryAgain);
			soundLetsTrySomethingDifferent.load(urlSoundLetsTrySomethingDifferent);
			soundWouldYouLikeToMatchThePictures.load(urlSoundWouldYouLikeToMatchThePictures);
			soundWouldYouLikeToMatchThePairs.load(urlSoundWouldYouLikeToMatchThePairs);
			
			currentLevel = difficultyLevel;
			
			if (currentLevel < 5) 
				soundWouldYouLikeToMatchThePictures.play();
			else 
				soundWouldYouLikeToMatchThePairs.play();
			
			images = new Array();
			loadedImages = new Array();
			mainImage = new Array();
			
			// match-the-pairs
			imagesPairs = new Array();
			loadedImagesPairs = new Array();
			mainImagePairs = new Array();
			clickedImagesPairs = new Array();
			childImagesPairs = new Array();
			clickedImagesPairsInstance = new Array();

			if (enableGameTimeout)
				gameTimeout = setTimeout(timedFunctionGame, timeoutValue);
		}
		
		// called externally by the Windows UserControl
		private function playMatchingGame():void {
			loadScreen();
		}
		
		// called externally by the Windows UserControl
		private function stopMatchingGame():void {
			SoundMixer.stopAll();
			clearTimeout(gameTimeout);
			
			urlSoundCorrect = null;
			urlSoundGoodJob = null;
			urlSoundWellDone = null;
			urlSoundTryAgain = null;
			urlSoundLetsTrySomethingDifferent = null;
			urlSoundWouldYouLikeToMatchThePictures = null;
			urlSoundWouldYouLikeToMatchThePairs = null;
			
			soundCorrect = null;
			soundGoodJob = null;
			soundWellDone = null;
			soundTryAgain = null;
			soundWouldYouLikeToMatchThePictures = null;
			soundWouldYouLikeToMatchThePairs = null;
			
			images = null;
			loadedImages = null;
			mainImage = null;
			
			imagesPairs = null;
			loadedImagesPairs = null;
			mainImagePairs = null;
			clickedImagesPairs = null;
			childImagesPairs = null;
			clickedImagesPairsInstance = null;
		}
		
		private function loadScreen():void {
			while (numChildren > 0) {
				removeChildAt(0);
			}
			
			clickCount = 0;
			numAttempts = 0;
			clearArrays();
			
			switch(currentLevel)
			{
				case 1:
					boardWidth = 1;
					boardHeight = 2;
					break;
				case 2:
					boardWidth = 2;
					boardHeight = 2;
					break;
				case 3:
					boardWidth = 2;
					boardHeight = 3;
					break;
				case 4:
					boardWidth = 3;
					boardHeight = 3;
					break;
				case 5:
					boardWidth = 4;
					boardHeight = 3;
					break;
			}
			
			if (currentLevel < 5) {
				loadImages();
				loadMainImage();
				drawLine();
			} else {
				soundWouldYouLikeToMatchThePairs.play();
				loadImagesMatchThePairs();
			}
		}
		
		private function loadImages():void {
			var query:XML;
			var queryText:String = "";
			
			images_xml = new XML(xmlShapes);
			query = images_xml.image.parent();
			
			for each (var elements:XML in query.*.name) {
				queryText += elements;
				images.push(elements);
			}

			// load images
			for (var i:uint = 0; i < boardWidth; i++) {
				for (var j:uint = 0; j < boardHeight; j++) {
					var urlReq:URLRequest;
					var imgLoader:Loader = new Loader();
					var r:uint = Math.floor(Math.random() * images.length);
					var url:String = images[r];
					
					loadedImages.push(images[r]);
					images.splice(r, 1);
					urlReq = new URLRequest(url);
					imgLoader.load(urlReq);
					imgLoader.contentLoaderInfo.addEventListener(Event.COMPLETE, imageLoaded);

					addChild(imgLoader);
					switch (currentLevel)
					{
						case 1:
							TweenLite.to(imgLoader, 1, {x:i * fourthStageWidth + halfStageWidth + fourthStageWidth, y:j * halfStageHeight + halfStageHeight / 2, alpha:1, ease:Back.easeOut});
							break;
						case 2:
							TweenLite.to(imgLoader, 1, {x:i * fourthStageWidth + halfStageWidth + fourthStageWidth / 2, y:j * halfStageHeight + halfStageHeight / 2, alpha:1, ease:Back.easeOut});
							break;
						case 3:
							TweenLite.to(imgLoader, 1, {x:i * fourthStageWidth + halfStageWidth + fourthStageWidth / 2, y:j * thirdStageHeight + thirdStageHeight / 2, alpha:1, ease:Back.easeOut});
							break;
						case 4:
							TweenLite.to(imgLoader, 1, {x:i * sixthStageWidth + halfStageWidth + sixthStageWidth / 2, y:j * thirdStageHeight + thirdStageHeight / 2, alpha:1, ease:Back.easeOut});
							break;
					}
					
					imgLoader.addEventListener(MouseEvent.CLICK, imgClicked);
				}
			}
		}
		
		private function imageLoaded(event:Event):void {
			var img:Bitmap = Bitmap(event.target.content);
			var imgWidthRatio:Number;
			var imgHeightRatio:Number;
			
			imgWidthRatio = img.width / img.height;
			imgHeightRatio = img.height / img.width;
			
			if (img.width >= img.height) {
				switch (currentLevel)
				{
					case 1:
					case 2:
						img.width = fourthStageWidth - sixthStageWidth / 3;
						break;
					case 3:
					case 4:
						img.width = sixthStageWidth - sixthStageWidth / 4;
						break;
					case 5:
						img.width = fourthStageWidth - fourthStageWidth / 2.3;
						break;
				}
				img.height = img.width * imgHeightRatio;
			} else {
				switch (currentLevel)
				{
					case 1:
					case 2:
						img.height = halfStageHeight - sixthStageHeight;
						break;
					case 3:
					case 4:
						img.height = thirdStageHeight - sixthStageHeight;
						break;
					case 5:
						img.height = thirdStageHeight - thirdStageHeight / 2.3;
						break;
				}
				img.width = img.height * imgWidthRatio;
			}
			img.x = img.width / 2 - img.width;
			img.y = img.height / 2 - img.height;
			img.smoothing = true;
		}
		
		private function drawLine():void {
			verticalLine.graphics.lineStyle(7, 0x000000, 1);
			verticalLine.graphics.moveTo(halfStageWidth, 0);
			verticalLine.graphics.lineTo(halfStageWidth, stageHeight);
			addChild(verticalLine);
		}
			
		private function imgClicked(event:MouseEvent):void {
			var fullPathMainImage:String = mainImage[0];
			var mainFilename:String = fullPathMainImage.substring(fullPathMainImage.lastIndexOf("\\") + 1, fullPathMainImage.length).toLowerCase();
			
			var fullPath:String = event.target.content.loaderInfo.url;
			
			var lastIndexOfSlash:Number = fullPath.lastIndexOf("\\");
			if (lastIndexOfSlash <= 0)
				lastIndexOfSlash = fullPath.lastIndexOf("/");
			
			var filename:String = fullPath.substring(lastIndexOfSlash + 1, fullPath.length).toLowerCase();
			
			clickCount++;
			
			// correct - go to next level
			if (mainFilename == filename) {
				setTimeout(timedFunctionImage, 3000);
				SoundMixer.stopAll();
				playCorrectSound();
				LogInteractiveActivityEvent(1, "Image '" + RemoveExtension(mainFilename) + "' was matched");
				
				TweenMax.to(event.target, 2, { transformMatrix:{a:0, tx:event.target.x, ty:event.target.y, scaleX:1.2, scaleY:1.2 }});
				if (currentLevel < 5) currentLevel++;
				
				function timedFunctionImage():void {
					if (currentLevel < 5) spliceArrays();
					loadScreen();
				}

			// incorrect - reload level 1 or go back a level
			} else if (clickCount == 1) {
				soundTryAgain.play();
				LogInteractiveActivityEvent(0, "Image '" + RemoveExtension(mainFilename) + "' was mismatched (Selected Image: '" + RemoveExtension(filename) + "')");
				spliceArrays()
				if (currentLevel > 1) currentLevel--;
				loadScreen();
			}
			
			if (enableGameTimeout) {
				clearTimeout(gameTimeout);
				gameTimeout = setTimeout(timedFunctionGame, timeoutValue);
			}
		}
		
		private function RemoveExtension(filename:String):String {
			var slashIndex:Number = filename.lastIndexOf("\\");
			var nameWithExtension:String = filename.substr( slashIndex + 1, filename.length );
			var extensionIndex:Number = nameWithExtension.lastIndexOf(".");
			var nameNoExtension:String = nameWithExtension.substr( 0, extensionIndex );
			
			return nameNoExtension;
		}
		
		private function clearArrays():void {
			images = [];
			loadedImages = [];
			mainImage = [];
			
			imagesPairs = [];
			loadedImagesPairs = [];
			mainImagePairs = [];
			clickedImagesPairs = [];
			clickedImagesPairsInstance = [];
		}
			
		// ----------------------- match-the-pairs (begin) -----------------------------
		
		private function loadImagesMatchThePairs():void {
			var query:XML;
			var queryText:String = "";
			
			images_xml = new XML(xmlShapes);
			query = images_xml.image.parent();
			
			for each (var elements:XML in query.*.name) {
				queryText += elements;
				imagesPairs.push(elements);
			}

			// load images
			for (var i:uint = 0; i < boardHeight * 2; i++) {
				var r:uint = Math.floor(Math.random() * imagesPairs.length);
				loadedImagesPairs.push(imagesPairs[r]);
				loadedImagesPairs.push(imagesPairs[r]);
				imagesPairs.splice(r, 1);
			}
			
			for (var f:uint = 0; f < boardWidth; f++) {
				for (var j:uint = 0; j < boardHeight; j++) {
					var urlReq:URLRequest;
					var imgLoader:Loader = new Loader();
					var fr:uint = Math.floor(Math.random() * loadedImagesPairs.length);
					var url:String = loadedImagesPairs[fr];
					
					mainImage.push(loadedImagesPairs[fr]);
					loadedImagesPairs.splice(fr,1);
					urlReq = new URLRequest(url);
					imgLoader.load(urlReq);
					imgLoader.contentLoaderInfo.addEventListener(Event.COMPLETE, imageLoaded);
					
					addChild(imgLoader);
					TweenLite.to(imgLoader, 1, {x:f * fourthStageWidth + fourthStageWidth / 2, y:j * thirdStageHeight / 1.2 + thirdStageHeight / 1.2, alpha:1, ease:Back.easeOut});

					imgLoader.addEventListener(MouseEvent.CLICK, imgMatchThePairsClicked);
				}
			}
			var matchThePairsText:Bitmap = getMatchThePairsText();
			addChild(matchThePairsText);
			TweenLite.to(matchThePairsText, 1, {x:thirdStageWidth, y:35, alpha:1});
			
			LogInteractiveActivityEvent(-1, "New set of pairs has been displayed (6 pairs)");
		}
		
		private function getMatchThePairsText():Bitmap {
			var findImg:Bitmap = TextAssets.matchThePairs;
			var findImgWidthRatio:Number;
			var findImgHeightRatio:Number;
			
			findImgWidthRatio = findImg.width / findImg.height;
			findImgHeightRatio = findImg.height / findImg.width;
			
			if (findImg.width >= findImg.height) {
				findImg.width = halfStageWidth - sixthStageWidth / 0.8;
                findImg.height = findImg.width * findImgHeightRatio;
			} else {
               findImg.height = halfStageHeight + sixthStageHeight / 0.8;
               findImg.width = findImg.height * findImgWidthRatio;
			}

			findImg.x = findImg.width / 2 - findImg.width;
			findImg.smoothing = true;
			
			return findImg;
		}
		
		private function imgMatchThePairsClicked(event:MouseEvent):void {
			clickedImagesPairs.push(event.target.content.loaderInfo.url);
			clickedImagesPairsInstance.push(event.target.name);
			childImagesPairs.push(event.target);
			
			var fullPath1:String = clickedImagesPairs[0];
			var fullPath2:String = clickedImagesPairs[1];
			var image1obj:String = clickedImagesPairsInstance[0];
			var image2obj:String = clickedImagesPairsInstance[1];
			
			var success:Number;
			var eventLogDescription:String;
			var isDecreaseDifficulty:Boolean;
			var filename1:String = fullPath1.substr(fullPath1.lastIndexOf("\\") + 1).toLowerCase();
			var filename2:String;
			
			if (fullPath2 != null)
				filename2 = fullPath2.substr(fullPath2.lastIndexOf("\\") + 1).toLowerCase();
			
			TweenMax.to(event.target, 0, {glowFilter:{color:0x0033cc, alpha:1, blurX:15, blurY:15, strength:4}});
			
			clickCount++;
			
			// correct
			if (image1obj != image2obj && filename1 == filename2) {
				success = 1;
				SoundMixer.stopAll();
				playCorrectSound();
				TweenMax.to(childImagesPairs[0], 1, {glowFilter:{color:0x0033cc, alpha:1, blurX:15, blurY:15, strength:4}});
				TweenMax.to(childImagesPairs[1], 1, {glowFilter:{color:0x0033cc, alpha:1, blurX:15, blurY:15, strength:4}});
				setTimeout(timedFunctionClick1, 200);
			
			// if wrong run again
			} else if (image1obj == image2obj && clickCount == 2) {
				success = 0;
				isDecreaseDifficulty = false;
				SoundMixer.stopAll();
				soundTryAgain.play();
				setTimeout(timedFunctionClick2, 100);

			} else if (image1obj != image2obj && clickCount == 2) {
				success = 0;
				isDecreaseDifficulty = false;
				SoundMixer.stopAll();
				soundTryAgain.play();
				setTimeout(timedFunctionClick3, 100);
			}
			
			if (image1obj != image2obj && filename1 == filename2 && numAttempts == 2) {
				success = 0;
				isDecreaseDifficulty = false;
				clickCount = 0;
				numAttempts = 0;
			} else if (image1obj != image2obj && clickCount == 2 && numAttempts == 2) {
				success = 0;
				isDecreaseDifficulty = true;
				SoundMixer.stopAll();
				soundLetsTrySomethingDifferent.play();
			} else if (image1obj == image2obj && clickCount == 2 && numAttempts == 2) {
				success = 0;
				isDecreaseDifficulty = true;
				SoundMixer.stopAll();
				soundLetsTrySomethingDifferent.play();
			}
			if (fullPath2 != null) {
				if (success == 1)
					eventLogDescription = "Pair for image '" + RemoveExtension(filename1) + "' was matched";
				else {
					eventLogDescription = "Pair for image '" + RemoveExtension(filename1) + "' was mismatched (Selected Image: '" + RemoveExtension(filename2) + "')";
					if (isDecreaseDifficulty) {
						currentLevel = 4
						loadScreen();
					}
				}
				LogInteractiveActivityEvent(success, eventLogDescription);
			}
			
			if (enableGameTimeout) {
				clearTimeout(gameTimeout);
				gameTimeout = setTimeout(timedFunctionGame, timeoutValue);
			}
		}
		
		private function timedFunctionClick1():void {
			clickCount = 0;
			numAttempts = 0;
			removeChild(childImagesPairs[0]);
			removeChild(childImagesPairs[1]);
			
			spliceArraysPairs();
			
			// game Over
			if (numChildren == 1) {
				while (numChildren > 0) {
					removeChildAt(0);
				}
				var gameOver:TextField = new TextField();
				var format:TextFormat = new TextFormat();
				format.color = 0x000000;
				format.size = 80;
				format.font="Arial";
				format.align=TextFormatAlign.CENTER;
				gameOver.defaultTextFormat=format;
				gameOver.text = "Well done - Game over ";
				gameOver.multiline = false;
				gameOver.width=halfStageWidth;
				gameOver.antiAliasType = AntiAliasType.ADVANCED;
				gameOver.sharpness = 100;
				gameOver.thickness = 100;
				gameOver.height = sixthStageWidth / 3;
				gameOver.x = halfStageWidth-gameOver.width / 2;
				gameOver.y = halfStageHeight-gameOver.height / 2;
				addChild(gameOver);
				LogInteractiveActivityEvent(1, "All pairs were matched successfully");
				setTimeout(timedFunctionClick3, 3000);
			}
		}
		
		private function timedFunctionClick2():void {			
			TweenMax.to(childImagesPairs[0], 1, {glowFilter:{color:0x0033cc, alpha:0, blurX:15, blurY:15, strength:4}});
			TweenMax.to(childImagesPairs[1], 1, {glowFilter:{color:0x0033cc, alpha:0, blurX:15, blurY:15, strength:4}});
			
			spliceArraysPairs();
			clickCount = 0;
			numAttempts++;
		}
		
		private function timedFunctionClick3():void {
			if (childImagesPairs.length > 0) {
				TweenMax.to(childImagesPairs[0], 1, {glowFilter:{color:0x0033cc, alpha:0, blurX:15, blurY:15, strength:4}});
				TweenMax.to(childImagesPairs[1], 1, {glowFilter:{color:0x0033cc, alpha:0, blurX:15, blurY:15, strength:4}});
				spliceArraysPairs();
				clickCount = 0;
				numAttempts++;
			} else {
				loadScreen();
			}
		}
			
		private function spliceArraysPairs():void {
			var image1:String = clickedImagesPairs[0];
			var image2:String = clickedImagesPairs[1];
			var image1obj:String = clickedImagesPairsInstance[0];
			var image2obj:String = clickedImagesPairsInstance[1];
			
			clickedImagesPairs.splice(image1, 1);
			clickedImagesPairs.splice(image2, 1);
			clickedImagesPairsInstance.splice(image1obj, 1);
			clickedImagesPairsInstance.splice(image2obj, 1);
			childImagesPairs.splice(image1, 1);
			childImagesPairs.splice(image2, 1);
		}
		
		// ----------------------  match-the-pairs (end) --------------------------------
		
		private function spliceArrays():void {
			images.splice(0);
			loadedImages.splice(0);
			mainImage.splice(0);
		}
		
		private function playCorrectSound():void {			
			switch(currentLevel)
			{
				case 1:
					soundCorrect.play();
					break;
				case 2:
					soundGoodJob.play();
					break;
				case 3:
					soundCorrect.play();
					break;
				case 4:
					soundGoodJob.play();
					break;
				case 5:
					soundWellDone.play();
					break;
			}
		}
		
		private function loadMainImage():void {
			var mainImgUrlReq:URLRequest;
			var mainImgLoader:Loader = new Loader();
			var fr:uint = Math.floor(Math.random() * loadedImages.length);
			var mainUrl:String = loadedImages[fr];
			var mainFilename:String = loadedImages[fr];
			
			mainImage.push(loadedImages[fr]);
			loadedImages.splice(fr, 1);
			mainImgUrlReq = new URLRequest(mainUrl);
			mainImgLoader.load(mainImgUrlReq);
			mainImgLoader.contentLoaderInfo.addEventListener(Event.COMPLETE, mainImageLoaded);
			
			addChild(mainImgLoader);
			TweenLite.to(mainImgLoader, 1, {x:fourthStageWidth, y:halfStageHeight, alpha:1});

			var matchThePicturesText:Bitmap = getMatchThePicturesText();
			addChild(matchThePicturesText);
			TweenLite.to(matchThePicturesText, 1, {x:120, y:100, alpha:1});
			
			LogInteractiveActivityEvent(-1, "New target image has been displayed (Image: '" + RemoveExtension(mainFilename) + "')");
		}
		
		private function mainImageLoaded(event:Event):void {
			var mainImg:Bitmap = Bitmap(event.target.content);
			var mainImgWidthRatio:Number;
			var mainImgHeightRatio:Number;
			
			mainImgWidthRatio = mainImg.width / mainImg.height;
			mainImgHeightRatio = mainImg.height / mainImg.width;
			
			if (mainImg.width >= mainImg.height) {
				mainImg.width = halfStageWidth - sixthStageWidth;
				mainImg.height = mainImg.width * mainImgHeightRatio;
			} else {
				mainImg.height = halfStageHeight + sixthStageWidth;
				mainImg.width = mainImg.height * mainImgWidthRatio;
			}

			mainImg.x = mainImg.width / 2 - mainImg.width;
			mainImg.y = mainImg.height / 2 - mainImg.height;
			mainImg.smoothing = true;
		}
		
		private function getMatchThePicturesText():Bitmap {
			var findImg:Bitmap = TextAssets.matchThePicture;
			var findImgWidthRatio:Number;
			var findImgHeightRatio:Number;
			
			findImgWidthRatio = findImg.width / findImg.height;
			findImgHeightRatio = findImg.height / findImg.width;
			
			if (findImg.width >= findImg.height) {
				findImg.width = halfStageWidth - sixthStageWidth;
				findImg.height = findImg.width * findImgHeightRatio;
			} else {
				findImg.height = halfStageHeight + sixthStageWidth;
				findImg.width = findImg.height * findImgWidthRatio;
			}

			findImg.x = findImg.width / 2 - findImg.width;
			findImg.y = findImg.height/ 2 - findImg.height;
			findImg.smoothing = true;
			
			return findImg;
		}
			
		private function timedFunctionGame():void {
			LogInteractiveActivityEvent(-1, "Game timeout has expired", true);
		}
	}
}