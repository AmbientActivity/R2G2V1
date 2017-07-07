package
{
	// R2G2 - need to add a library for ExternalInterface
	import flash.external.ExternalInterface;
	
	import com.Balloon;
	
	import flash.media.Sound;
    import flash.media.SoundChannel; 
    import flash.net.URLRequest;
    import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.display.*;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.TimerEvent;
	import flash.text.TextField;
	import flash.text.TextFormat;
	import flash.utils.Timer;
    import flash.system.fscommand;
	import flash.utils.setTimeout;
	import flash.utils.clearTimeout;
	import flash.events.TouchEvent

	[SWF(backgroundColor='#FFFF00', frameRate='60' )]

	public class Main extends MovieClip 
	{		
		// embedded images
		[Embed(source="../lib/start.png")]
		private var StartButton:Class;
		private var startButton:Sprite = new Sprite();

		[Embed(source="../lib/play-again.png")]
		private var RestartButton:Class;
		private var restartButton:Sprite = new Sprite();
		
		// R2G2 - all resources need to be embedded
		// embedded sounds
		[Embed(source='/../lib/sounds/pop0.mp3')]
		private var SoundPop0 : Class;
		[Embed(source='/../lib/sounds/pop1.mp3')]
		private var SoundPop1 : Class;
		[Embed(source='/../lib/sounds/pop2.mp3')]
		private var SoundPop2 : Class;
		[Embed(source='/../lib/sounds/pop3.mp3')]
		private var SoundPop3 : Class;
		[Embed(source='/../lib/sounds/pop4.mp3')]
		private var SoundPop4 : Class;
		[Embed(source='/../lib/sounds/pop5.mp3')]
		private var SoundPop5 : Class;
		[Embed(source='/../lib/sounds/pop6.mp3')]
		private var SoundPop6 : Class;
		[Embed(source='/../lib/sounds/pop7.mp3')]
		private var SoundPop7 : Class;
		
		private var balloons:Array = [];
		private var timer:Timer = new Timer(10000, -1);
		private var score:int;
		private var textBox:TextField = new TextField;
		private var textFormat:TextFormat = new TextFormat();		
		
		// R2G2 - timeout and enable/disable boolean
		// When triggered by a phidget - timeout
		// When triggered by the Caregiver Interface - no timeout
		private var activityTimeout:uint;
		private var timeoutValue:Number = 900000;  // 15 minutes
		private var enableActivityTimeout:Boolean;
		
		public function Main():void {
			
			// R2G2 - need to add 2 entry points
			// comment to test
			ExternalInterface.addCallback("playActivity", playActivity);
			ExternalInterface.addCallback("stopActivity", stopActivity);
			
			// uncomment to run locally
			//playActivity(0);
		}
		
		// R2G2 - function to call back to Display App for logging an event
		private function LogInteractiveActivityEvent(description:String, isGameHasExpired:Boolean = false):void {
			// comment the following line to test
			ExternalInterface.call("FlashCall", description, isGameHasExpired);
		}
		
		// R2G2 - need to add function for the entry point "playActivity"
		// init() gets called from here instead of from Main()
		// receives a parameter to determine if the game should timeout on or off
		// doesn't accept booleans properly so convert number to boolean
		private function playActivity(enableTimeout:Number):void {
			enableActivityTimeout = (enableTimeout == 1);
			
			// R2G2 - need to set the timer for the beginning of the game
			setTimer();
			
			init();
		}
		
		// R2G2 - function for the entry point "stopActivity"
		private function stopActivity():void {
			
			// clear the timeout
			clearTimeout(activityTimeout);
			
			// remove all children
			while (numChildren > 0) {
				removeChildAt(0);
			}
			
			// empty the balloons
			balloons = [];
		}
		
		private function init(e:Event = null):void {
			
			//add start button
			startButton.addChild(new StartButton());
			startButton.addEventListener(MouseEvent.CLICK, startGame);
			startButton.buttonMode = true;
			startButton.x = (stage.stageWidth / 2) - (startButton.width / 2);
			startButton.y = (stage.stageHeight / 2);
			addChild(startButton);
			
			//instantiate the restart button
			restartButton.addChild(new RestartButton());
			restartButton.buttonMode = true;
			restartButton.x = (stage.stageWidth / 2) - (restartButton.width / 2);
			restartButton.y = (stage.stageHeight / 2);
			
			textFormat.font = "Arial";
			textFormat.size = 80;
			textFormat.align = "center";
			textFormat.letterSpacing = 3;
			textFormat.leading = 50;
			textBox.defaultTextFormat = textFormat;
			textBox.text = "\"Pop The Balloons\"\nTouch the Start button to play the game.";
            textBox.width = textBox.textWidth;
			textBox.height = stage.stageHeight / 4;
			textBox.x = (stage.stageWidth / 2) - (textBox.width / 2);
			textBox.y = (stage.stageHeight / 4) - (textBox.height / 2);
			addChild(textBox);
		}
		
		private function balloonCheck(e:Event):void {
			if (balloons.length)
			{
				for (var i:int = 0; i < balloons.length; i++)
				{
					if (balloons[i].y == 0 - balloons[i].height)
					{
						removeEventListener(Event.ENTER_FRAME, balloonCheck);
						for (var j:int = 0; j < balloons.length; j++)
						{
							balloons[j].die();
							removeChild(balloons[j]);
						}
						timer.stop();
						textBox.text = "You popped " + score + " balloons. \n Well Done!";
						textBox.width = textBox.textWidth;
						textBox.x = (stage.stageWidth / 2) - (textBox.width / 2);
						textBox.y = (stage.stageHeight / 4) - (textBox.height / 2);
						addChild(textBox);
						
						restartButton.addEventListener(MouseEvent.CLICK, restartGame);
						addChild(restartButton);
						
						// R2G2 - reset the timer
						setTimer();
						
						// R2G2 - log event reporting the final score
						LogInteractiveActivityEvent("Game over. " + score + " balloon(s) popped", false)
			
						return;
					}
				}
			}
		}
		
		private function restartGame(e:MouseEvent):void {
			// R2G2 - need to reset the timer when a new is game started
			setTimer();
			
			// R2G2 - log event reporting that a new game was started
			LogInteractiveActivityEvent("New game initiated", false)
						
			balloons = [];
			restartButton.removeEventListener(MouseEvent.CLICK, restartGame);
			removeChild(restartButton);
			removeChild(textBox);
			
			textBox.defaultTextFormat = textFormat;
			textBox.text = "Touch a balloon";
			addChild(textBox);
			game();
		}

		private function startGame(e:MouseEvent):void {
			// R2G2 - log event reporting that a new game was started
			LogInteractiveActivityEvent("New game initiated", false)
			
			// R2G2 - need to reset the timer when a new is game started
			setTimer();
			
			startButton.removeEventListener(MouseEvent.CLICK, startGame);
			removeChild(startButton);
			removeChild(textBox);

			textBox.defaultTextFormat = textFormat;
			textBox.text = "Touch a balloon";
			addChild(textBox);
			game();
		}
		
		private function game():void {
			addEventListener(Event.ENTER_FRAME, balloonCheck);
			timer.addEventListener(TimerEvent.TIMER_COMPLETE, createBalloon);
			timer.start();
			createBalloon();
			score = 0;
		}
		
		private function createBalloon(e:TimerEvent = null):void {
			var balloon:Balloon = new Balloon();
			balloon.addEventListener(MouseEvent.CLICK, popBalloon);
			balloon.addEventListener(MouseEvent.CLICK, playsound);  
			balloon.y = stage.stageHeight;
			balloon.x = Math.floor(Math.random() * (stage.stageWidth - balloon.width));
			balloons.push(balloon);
			addChild(balloon);
			timer.reset();
			timer.start();
		}
		
		private function popBalloon(e:MouseEvent):void {
			e.target.x = Math.floor(Math.random() * (stage.stageWidth - e.target.width));
			e.target.reset();
			score++;
			
			// R2G2 - log event that a balloon was popped
			LogInteractiveActivityEvent("A balloon was popped", false)
			
			// R2G2 - reset the timeout
			setTimer();
		}

		private function playsound(e:MouseEvent):void {
			
			// R2G2 - need to access the sounds as embedded resources
			var sound:Sound;
			var randnum:uint;
			
			randnum = Math.floor(Math.random() * 7);
			switch(randnum)
			{
				case 1:
					sound = new SoundPop1();
					break;
				case 2:
					sound = new SoundPop2();
					break;
				case 3:
					sound = new SoundPop3();
					break;
				case 4:
					sound = new SoundPop4();
					break;
				case 5:
					sound = new SoundPop5();
					break;
				case 6:
					sound = new SoundPop6();
					break;
				case 7:
					sound = new SoundPop7();
					break;
				default:
					sound = new SoundPop0();
			}
			
			sound.play();
        }
		
		// R2G2 - function to reset the timer
		private function setTimer():void {
			if (enableActivityTimeout) {
				clearTimeout(activityTimeout);
				activityTimeout = setTimeout(timedFunctionGame, timeoutValue);
			}
		}
		
		// R2G2 - function to notify the Display App that the timeout has expired
		// informing it that its timeout has expired and it can now be closed
		private function timedFunctionGame():void {
			stopActivity();
			LogInteractiveActivityEvent("Game timeout has expired", true);
		}
    }
}
