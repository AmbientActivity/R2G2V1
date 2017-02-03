package
{
	//import flash.external.ExternalInterface;
	//import flash.display.Sprite;
	//import flash.events.Event;
	//import flash.utils.setTimeout;
	//import flash.utils.clearTimeout;
	
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
	
	[SWF(backgroundColor="0xffff00")]
	public class Main extends Sprite 
	{		
		private var gameTypeId:uint = 3;
		private var gameTimeout:uint;
		
		private var stageWidth:uint = 1920;
		private var stageHeight:uint = 1080;
		private var timeoutValue:Number = 900000;
				
		private var enableGameTimeout:Boolean;
		
		public function Main() {
			// comment to test
			ExternalInterface.addCallback("playPaintingGame", playPaintingGame);
			ExternalInterface.addCallback("stopPaintingGame", stopPaintingGame);
			
			// uncomment to test
			//playPaintingGame(1);
		}
		
		// called externally by the Windows UserControl
		private function playPaintingGame(enableTimeout:Number):void {
			//enableGameTimeout = (enableTimeout == 1);
			
			//if (enableGameTimeout)
			//	gameTimeout = setTimeout(timedFunctionGame, timeoutValue);
		}
		
		// called externally by the Windows UserControl
		private function stopPaintingGame():void {
			clearTimeout(gameTimeout);
		}
		
		private function LogGamingEvent(description:String, isGameHasExpired:Boolean = false):void {
			
			// comment the following line to test
			ExternalInterface.call("FlashCall", gameTypeId, description, isGameHasExpired);
		}
		
		private function timedFunctionGame():void {
			LogGamingEvent("Game timeout has expired", true);
		}
	}
}