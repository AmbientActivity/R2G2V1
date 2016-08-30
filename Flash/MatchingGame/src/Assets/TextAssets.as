package Assets
{
	import flash.display.Bitmap;
	
	/**
	 * ...
	 * @author John Charlton
	 */
	
	public class TextAssets 
	{
		[Embed(source = "images/match-the-picture.png")]
		private static var MatchThePicture:Class;
		
		[Embed(source = "images/match-the-pairs.png")]
		private static var MatchThePairs:Class;
		
		public static var matchThePicture:Bitmap;
		public static var matchThePairs:Bitmap;

		public static function init():void
		{
			matchThePicture = (new MatchThePicture() as Bitmap);
			matchThePairs = (new MatchThePairs() as Bitmap);
		}
	}
}