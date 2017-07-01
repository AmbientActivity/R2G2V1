package
{
	import flash.external.ExternalInterface;
	import com.flashandmath.dg.GUI.GradientSwatch;
	import flash.display.Sprite;
	
	import flash.net.URLRequest;
	import flash.net.URLLoader;
	import flash.media.SoundMixer;
	import flash.media.Sound;
	import flash.display.*;
	import flash.display.SimpleButton;
	import flash.display.Sprite;
	import flash.display.Shape;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.KeyboardEvent;
	import flash.events.IOErrorEvent;
	import flash.display.Loader;
	import flash.display.Bitmap;
	import flash.display.BitmapData;
	import flash.utils.setTimeout;
	import flash.utils.clearTimeout;
	
	import flash.text.*;
	import flash.text.TextFormat;
	import flash.text.TextFormatAlign;
	import flash.text.AntiAliasType;
	import flash.geom.Matrix;
	import flash.geom.Point;
	
	import com.flashandmath.dg.bitmapUtilities.BitmapSaver;
	
	[SWF(backgroundColor = "0x000000")]
	public class Main extends Sprite
	{
		private var gameTypeId:Number = 3;
		
		private var lineLayer:Sprite;
		private var lastSmoothedMouseX:Number;
		private var lastSmoothedMouseY:Number;
		private var lastMouseX:Number;
		private var lastMouseY:Number;
		private var lastThickness:Number;
		private var lastRotation:Number;
		private var lineColor:uint;
		private var lineThickness:Number;
		private var lineRotation:Number;
		private var L0Sin0:Number;
		private var L0Cos0:Number;
		private var L1Sin1:Number;
		private var L1Cos1:Number;
		private var sin0:Number;
		private var cos0:Number;
		private var sin1:Number;
		private var cos1:Number;
		private var dx:Number;
		private var dy:Number;
		private var dist:Number;
		private var targetLineThickness:Number;
		private var colorLevel:Number;
		private var targetColorLevel:Number;
		private var smoothedMouseX:Number;
		private var smoothedMouseY:Number;
		private var tipLayer:Sprite;
		private var boardBitmap:Bitmap;
		private var boardBitmapData:BitmapData;
		private var bitmapHolder:Sprite;
		private var boardWidth:Number;
		private var boardHeight:Number;
		private var smoothingFactor:Number;
		private var mouseMoved:Boolean;
		private var dotRadius:Number;
		private var startX:Number;
		private var startY:Number;
		private var undoStack:Vector.<BitmapData>;
		private var minThickness:Number;
		private var thicknessFactor:Number;
		private var mouseChangeVectorX:Number;
		private var mouseChangeVectorY:Number;
		private var lastMouseChangeVectorX:Number;
		private var lastMouseChangeVectorY:Number;
		
		private var thicknessSmoothingFactor:Number;
		
		//var bitmapSaver:BitmapSaver;
		
		private var controlVecX:Number;
		private var controlVecY:Number;
		private var controlX1:Number;
		private var controlY1:Number;
		private var controlX2:Number;
		private var controlY2:Number;
		
		private var tipTaperFactor:Number;
		
		private var numUndoLevels:Number;
		
		private var controlPanel:Sprite;
		private var swatches:Vector.<GradientSwatch>;
		private var swatchColors:Vector.<uint>;
		
		private var paintColorR1:Number;
		private var paintColorG1:Number;
		private var paintColorB1:Number;
		private var paintColorR2:Number;
		private var paintColorG2:Number;
		private var paintColorB2:Number;
		
		private var red:Number;
		private var green:Number;
		private var blue:Number;
		
		private var colorChangeRate:Number;
		
		private var panelColor:uint;
		
		private var boardMask:Sprite;
		
		private var activityTimeout:uint;
		private var timeoutValue:Number = 900000;
		private var enableActivityTimeout:Boolean;
		private var currentSwatch:Object;
		private var currentColourSelection:String;
		
		//Setting the following NO_SCALE parameter helps avoid strange artifacts
		//in the displayed bitmaps caused by repositioning of the swf within the html page.
		//stage.scaleMode=StageScaleMode.NO_SCALE;
		
		public function Main()
		{
			// comment to test
			ExternalInterface.addCallback("playActivity", playPaintingActivity);
			ExternalInterface.addCallback("stopActivity", stopPaintingActivity);
			
			stage.displayState = StageDisplayState.FULL_SCREEN;
			//playPaintingActivity(1);
		}
		
		// called externally by the Windows UserControl
		private function playPaintingActivity(enableTimeout:Number):void
		{
			enableActivityTimeout = (enableTimeout == 1);
			
			if (enableActivityTimeout)
				activityTimeout = setTimeout(timedFunctionGame, timeoutValue);
			
			init();
		}
		
		// called externally by the Windows UserControl
		private function stopPaintingActivity():void
		{
			clearTimeout(activityTimeout);
		}
		
		private function init():void
		{
			boardWidth = 1920 /*1280*/;
			boardHeight = 978  /*718*/;
			
			minThickness = 0.2;
			thicknessFactor = 0.7;
			
			smoothingFactor = 0.3;  //Should be set to something between 0 and 1.  Higher numbers mean less smoothing.
			thicknessSmoothingFactor = 0.3;
			
			dotRadius = 2; //radius for drawn dot if there is no mouse movement between mouse down and mouse up.
			
			tipTaperFactor = 0.8;
			
			numUndoLevels = 50;
			
			colorChangeRate = 0.05;
			
			panelColor = 0xC6B689;
			
			paintColorR1 = 0;
			paintColorG1 = 0;
			paintColorB1 = 0;
			paintColorR2 = 0;
			paintColorG2 = 0;
			paintColorB2 = 0;
			
			swatchColors = Vector.<uint>([darkenColor(0x0000FF, 0.7), 0x0000FF, darkenColor(0x996633, 0.7), 0x996633, darkenColor(0x00FFFF, 0.7), 0x00FFFF, darkenColor(0x33CC00, 0.7), 0x33CC00, darkenColor(0xFF00FF, 0.7), 0xFF00FF, darkenColor(0xFF7F00, 0.7), 0xFF7F00, darkenColor(0x7F007F, 0.7), 0x7F007F, darkenColor(0xFF0000, 0.7), 0xFF0000, darkenColor(0xFFFF00, 0.7), 0xFFFF00, 0x000000, 0x000000, darkenColor(0x555555, 0.7), 0x555555, darkenColor(0x999999, 0.7), 0x999999, darkenColor(0xFFFFFF, 0.9), 0xFFFFFF]);
			swatches = new Vector.<GradientSwatch>;
			
			boardBitmapData = new BitmapData(boardWidth, boardHeight, false);
			boardBitmap = new Bitmap(boardBitmapData);
			
			//The undo buffer will hold the previous drawing.
			//If we want more levels of undo, we would have to record several undo buffers.  We only use one
			//here for simplicity.
			undoStack = new Vector.<BitmapData>;
			bitmapHolder = new Sprite();
			lineLayer = new Sprite();
			
			boardMask = new Sprite();
			boardMask.graphics.beginFill(0xFF0000);
			boardMask.graphics.drawRect(0, 0, boardWidth, boardHeight);
			boardMask.graphics.endFill();
			
			drawBackground();
			
			/*
			   The tipLayer holds the tip portion of the line.
			   Because of the smoothing technique we are using, while the user is drawing the drawn line will not
			   extend all the way from the last position to the current mouse position.  We use a small 'tip' to
			   complete this line all the way to the current mouse position.
			 */
			tipLayer = new Sprite();
			tipLayer.mouseEnabled = false;
			
			/*
			   Bitmaps cannot receive mouse events.  so we add it to a holder sprite.
			 */
			this.addChild(bitmapHolder);
			bitmapHolder.x = 0;
			bitmapHolder.y = 0;
			bitmapHolder.addChild(boardBitmap);
			bitmapHolder.addChild(tipLayer);
			bitmapHolder.addChild(boardMask);
			bitmapHolder.mask = boardMask;
			
			//We add the panel at the bottom which will hold color swatches
			controlPanel = new Sprite();
			controlPanel.graphics.beginFill(panelColor);
			controlPanel.graphics.drawRect(0, 0, boardWidth, 100);
			controlPanel.graphics.endFill();
			controlPanel.x = bitmapHolder.x;
			controlPanel.y = bitmapHolder.y + boardHeight + 2;
			this.addChild(controlPanel);
			createSwatches();
			
			var paintBg:Sprite = new Sprite();
			paintBg.graphics.beginFill(0xEEEEEE);
			paintBg.graphics.drawRect(0, 0, stage.stageWidth, 100);
			paintBg.graphics.endFill();
			
			var txtPaint:TextField = new TextField();
			txtPaint.text = "Paint with your finger on the screen";
			txtPaint.x = stage.stageWidth / 2 - paintBg.width / 2;
			txtPaint.y = 0;
			txtPaint.width = stage.stageWidth;
			txtPaint.height = 100;
			
			var tfPaint:TextFormat = new TextFormat();
			tfPaint.color = 0x000000;
			tfPaint.font = "Verdana";
			tfPaint.size = 70;
			tfPaint.align = "center";
			txtPaint.setTextFormat(tfPaint);
			txtPaint.selectable = false;
			
			var mcPaint:MovieClip = new MovieClip();
			mcPaint.addChild(paintBg);
			mcPaint.addChild(txtPaint);
			addChild(mcPaint);
			
			//Buttons
			var btnbgUndo:Sprite = new Sprite();
			btnbgUndo.graphics.beginFill(0xEEEEEE);
			btnbgUndo.graphics.drawRoundRect(0, 0, 140, 80, 30, 30);
			btnbgUndo.graphics.endFill();
			
			var btnbgErase:Sprite = new Sprite();
			btnbgErase.graphics.beginFill(0xEEEEEE);
			btnbgErase.graphics.drawRoundRect(0, 0, 140, 80, 30, 30);
			btnbgErase.graphics.endFill();
			
			var tfErase:TextFormat = new TextFormat();
			tfErase.color = 0x000000;
			tfErase.font = "Verdana";
			tfErase.size = 40;
			tfErase.align = "center";
			
			var tfUndo:TextFormat = new TextFormat();
			tfUndo.color = 0x000000;
			tfUndo.font = "Verdana";
			tfUndo.size = 40;
			tfUndo.align = "center";
			
			var txtErase:TextField = new TextField();
			txtErase.text = "erase";
			txtErase.x = 0;
			txtErase.y = 10;
			txtErase.width = btnbgErase.width;
			txtErase.height = btnbgErase.height;
			txtErase.setTextFormat(tfErase);
			
			var txtUndo:TextField = new TextField();
			txtUndo.text = "undo";
			txtUndo.x = 0;
			txtUndo.y = 10;
			txtUndo.width = btnbgUndo.width;
			txtUndo.height = btnbgUndo.height;
			txtUndo.setTextFormat(tfUndo);
			
			var mcUndo:MovieClip = new MovieClip();
			mcUndo.addChild(btnbgUndo);
			mcUndo.addChild(txtUndo);
			
			var mcErase:MovieClip = new MovieClip();
			mcErase.addChild(btnbgErase);
			mcErase.addChild(txtErase);
			
			var btnErase:SimpleButton = new SimpleButton();
			btnErase.upState = mcErase;
			btnErase.overState = mcErase;
			btnErase.downState = mcErase;
			btnErase.hitTestState = btnErase.upState;
			btnErase.x = stage.stageWidth - btnErase.width*2.5;
			btnErase.y = stage.stageHeight - btnErase.height;
			btnErase.addEventListener(MouseEvent.MOUSE_DOWN, erase);
			addChild(btnErase);
			
			var btnUndo:SimpleButton = new SimpleButton();
			btnUndo.upState = mcUndo;
			btnUndo.overState = mcUndo;
			btnUndo.downState = mcUndo;
			btnUndo.hitTestState = btnUndo.upState;
			btnUndo.x = btnErase.x - btnUndo.width - 20;
			btnUndo.y = btnErase.y;
			btnUndo.addEventListener(MouseEvent.MOUSE_DOWN, undo);
			addChild(btnUndo);
			
			bitmapHolder.addEventListener(MouseEvent.MOUSE_DOWN, startDraw);
			
			currentColourSelection = "Black"
			LogInteractiveActivityEvent("New palette has been created (Brush colour: " + currentColourSelection + ")", false)
		}
		
		private function createSwatches():void
		{
			var swatchLength:Number = Math.floor(0.8 * controlPanel.height);
			var space:Number = 5;
			for (var i:Number = 0; i < swatchColors.length; i = i + 2)
			{
				var thisSwatch:GradientSwatch = new GradientSwatch(swatchColors[i], swatchColors[i + 1], 1 * swatchLength, swatchLength);
				thisSwatch.x = (space + 1 * swatchLength) * (i + 1) / 2;
				thisSwatch.y = controlPanel.height / 2;
				controlPanel.addChild(thisSwatch);
				swatches.push(thisSwatch);
				thisSwatch.addEventListener(MouseEvent.CLICK, swatchClickHandler);
			}
		}
		
		private function swatchClickHandler(evt:MouseEvent):void
		{
			if (enableActivityTimeout)
			{
				clearTimeout(activityTimeout);
				activityTimeout = setTimeout(timedFunctionGame, timeoutValue);
			}
			
			currentSwatch = evt.currentTarget;
			var thisSwatch:Object = evt.currentTarget;
			paintColorR1 = thisSwatch.red1;
			paintColorG1 = thisSwatch.green1;
			paintColorB1 = thisSwatch.blue1;
			paintColorR2 = thisSwatch.red2;
			paintColorG2 = thisSwatch.green2;
			paintColorB2 = thisSwatch.blue2;
			
			currentColourSelection = GetColourDescriptor(thisSwatch)
			LogInteractiveActivityEvent("New colour was selected (" + currentColourSelection + ")")
		}
		
		private function startDraw(evt:MouseEvent):void
		{
			if (enableActivityTimeout)
			{
				clearTimeout(activityTimeout);
				activityTimeout = setTimeout(timedFunctionGame, timeoutValue);
			}
			
			stage.addEventListener(MouseEvent.MOUSE_UP, stopDraw);
			
			startX = lastMouseX = smoothedMouseX = lastSmoothedMouseX = bitmapHolder.mouseX;
			startY = lastMouseY = smoothedMouseY = lastSmoothedMouseY = bitmapHolder.mouseY;
			lastThickness = 0;
			lastRotation = Math.PI / 2;
			colorLevel = 0;
			lastMouseChangeVectorX = 0;
			lastMouseChangeVectorY = 0;
			
			//We will keep track of whether the mouse moves in between a mouse down and a mouse up.  If not,
			//a small dot will be drawn.
			mouseMoved = false;
			
			stage.addEventListener(MouseEvent.MOUSE_MOVE, drawLine);
			//this.addEventListener(Event.ENTER_FRAME, drawLine);
		}
		
		private function drawLine(evt:MouseEvent):void
		{
			mouseMoved = true;
			
			lineLayer.graphics.clear();
			
			mouseChangeVectorX = bitmapHolder.mouseX - lastMouseX;
			mouseChangeVectorY = bitmapHolder.mouseY - lastMouseY;
			
			//Cusp detection - if the mouse movement is more than 90 degrees
			//from the last motion, we will draw all the way out to the last
			//mouse position before proceeding.  We handle this by drawing the
			//previous tipLayer, and resetting the last smoothed mouse position
			//to the last actual mouse position.
			//We use a dot product to determine whether the mouse movement is
			//more than 90 degrees from the last motion.
			if (mouseChangeVectorX * lastMouseChangeVectorX + mouseChangeVectorY * lastMouseChangeVectorY < 0)
			{
				boardBitmapData.draw(tipLayer);
				smoothedMouseX = lastSmoothedMouseX = lastMouseX;
				smoothedMouseY = lastSmoothedMouseY = lastMouseY;
				lastRotation += Math.PI;
				lastThickness = tipTaperFactor * lastThickness;
			}
			
			//We smooth out the mouse position.  The drawn line will not extend to the current mouse position; instead
			//it will be drawn only a portion of the way towards the current mouse position.  This creates a nice
			//smoothing effect.
			smoothedMouseX = smoothedMouseX + smoothingFactor * (bitmapHolder.mouseX - smoothedMouseX);
			smoothedMouseY = smoothedMouseY + smoothingFactor * (bitmapHolder.mouseY - smoothedMouseY);
			
			//We determine how far the mouse moved since the last position.  We use this distance to change
			//the thickness and brightness of the line.
			dx = smoothedMouseX - lastSmoothedMouseX;
			dy = smoothedMouseY - lastSmoothedMouseY;
			dist = Math.sqrt(dx * dx + dy * dy);
			
			if (dist != 0)
			{
				lineRotation = Math.PI / 2 + Math.atan2(dy, dx);
			}
			else
			{
				lineRotation = 0;
			}
			
			//We use a similar smoothing technique to change the thickness of the line, so that it doesn't
			//change too abruptly.
			targetLineThickness = minThickness + thicknessFactor * dist;
			lineThickness = lastThickness + thicknessSmoothingFactor * (targetLineThickness - lastThickness);
			
			/*
			   The "line" being drawn is actually composed of filled in shapes.  This is what allows
			   us to create a varying thickness of the line.
			 */
			sin0 = Math.sin(lastRotation);
			cos0 = Math.cos(lastRotation);
			sin1 = Math.sin(lineRotation);
			cos1 = Math.cos(lineRotation);
			L0Sin0 = lastThickness * sin0;
			L0Cos0 = lastThickness * cos0;
			L1Sin1 = lineThickness * sin1;
			L1Cos1 = lineThickness * cos1;
			targetColorLevel = Math.min(1, colorChangeRate * dist);
			colorLevel = colorLevel + 0.2 * (targetColorLevel - colorLevel);
			
			red = paintColorR1 + colorLevel * (paintColorR2 - paintColorR1);
			green = paintColorG1 + colorLevel * (paintColorG2 - paintColorG1);
			blue = paintColorB1 + colorLevel * (paintColorB2 - paintColorB1);
			
			lineColor = (red << 16) | (green << 8) | (blue);
			
			controlVecX = 0.33 * dist * sin0;
			controlVecY = -0.33 * dist * cos0;
			controlX1 = lastSmoothedMouseX + L0Cos0 + controlVecX;
			controlY1 = lastSmoothedMouseY + L0Sin0 + controlVecY;
			controlX2 = lastSmoothedMouseX - L0Cos0 + controlVecX;
			controlY2 = lastSmoothedMouseY - L0Sin0 + controlVecY;
			
			lineLayer.graphics.lineStyle(1, lineColor);
			lineLayer.graphics.beginFill(lineColor);
			lineLayer.graphics.moveTo(lastSmoothedMouseX + L0Cos0, lastSmoothedMouseY + L0Sin0);
			lineLayer.graphics.curveTo(controlX1, controlY1, smoothedMouseX + L1Cos1, smoothedMouseY + L1Sin1);
			lineLayer.graphics.lineTo(smoothedMouseX - L1Cos1, smoothedMouseY - L1Sin1);
			lineLayer.graphics.curveTo(controlX2, controlY2, lastSmoothedMouseX - L0Cos0, lastSmoothedMouseY - L0Sin0);
			lineLayer.graphics.lineTo(lastSmoothedMouseX + L0Cos0, lastSmoothedMouseY + L0Sin0);
			lineLayer.graphics.endFill();
			boardBitmapData.draw(lineLayer);
			
			//We draw the tip, which completes the line from the smoothed mouse position to the actual mouse position.
			//We won't actually add this to the drawn bitmap until a mouse up completes the drawing of the current line.
			
			//round tip:
			var taperThickness:Number = tipTaperFactor * lineThickness;
			tipLayer.graphics.clear();
			tipLayer.graphics.beginFill(lineColor);
			tipLayer.graphics.drawEllipse(bitmapHolder.mouseX - taperThickness, bitmapHolder.mouseY - taperThickness, 2 * taperThickness, 2 * taperThickness);
			tipLayer.graphics.endFill();
			//quad segment
			tipLayer.graphics.lineStyle(1, lineColor);
			tipLayer.graphics.beginFill(lineColor);
			tipLayer.graphics.moveTo(smoothedMouseX + L1Cos1, smoothedMouseY + L1Sin1);
			tipLayer.graphics.lineTo(bitmapHolder.mouseX + tipTaperFactor * L1Cos1, bitmapHolder.mouseY + tipTaperFactor * L1Sin1);
			tipLayer.graphics.lineTo(bitmapHolder.mouseX - tipTaperFactor * L1Cos1, bitmapHolder.mouseY - tipTaperFactor * L1Sin1);
			tipLayer.graphics.lineTo(smoothedMouseX - L1Cos1, smoothedMouseY - L1Sin1);
			tipLayer.graphics.lineTo(smoothedMouseX + L1Cos1, smoothedMouseY + L1Sin1);
			tipLayer.graphics.endFill();
			
			lastSmoothedMouseX = smoothedMouseX;
			lastSmoothedMouseY = smoothedMouseY;
			lastRotation = lineRotation;
			lastThickness = lineThickness;
			lastMouseChangeVectorX = mouseChangeVectorX;
			lastMouseChangeVectorY = mouseChangeVectorY;
			lastMouseX = bitmapHolder.mouseX;
			lastMouseY = bitmapHolder.mouseY;
			
			evt.updateAfterEvent();
		
		}
		
		private function stopDraw(evt:MouseEvent):void
		{
			//If the mouse didn't move, we will draw just a dot.  Its size will be randomized.
			if (!mouseMoved)
			{
				var randRadius:Number = dotRadius * (0.75 + 0.75 * Math.random());
				var dotColor:uint = (paintColorR1 << 16) | (paintColorG1 << 8) | (paintColorB1);
				var dot:Sprite = new Sprite();
				dot.graphics.beginFill(dotColor)
				dot.graphics.drawEllipse(startX - randRadius, startY - randRadius, 2 * randRadius, 2 * randRadius);
				dot.graphics.endFill();
				boardBitmapData.draw(dot);
			}
			
			stage.removeEventListener(MouseEvent.MOUSE_MOVE, drawLine);
			stage.removeEventListener(MouseEvent.MOUSE_UP, stopDraw);
			
			//We add the tipLayer to complete the line all the way to the current mouse position:
			boardBitmapData.draw(tipLayer);
			
			//record undo bitmap and add to undo stack
			var undoBuffer:BitmapData = new BitmapData(boardWidth, boardHeight, false);
			undoBuffer.copyPixels(boardBitmapData, undoBuffer.rect, new Point(0, 0));
			undoStack.push(undoBuffer);
			if (undoStack.length > numUndoLevels + 1)
			{
				undoStack.splice(0, 1);
			}
			
			LogInteractiveActivityEvent("New brush stroke was applied", false)
		}
		
		private function erase(evt:MouseEvent):void
		{
			tipLayer.graphics.clear();
			drawBackground();
			
			LogInteractiveActivityEvent("The eraser was applied", false)
		}
		
		private function drawBackground():void
		{
			//We draw a background with a very subtle gradient effect so that the canvas darkens towards the edges.
			var gradMat:Matrix = new Matrix();
			gradMat.createGradientBox(1920, 998 /*1280,718*/, 0, 0, 0);
			var bg:Sprite = new Sprite();
			bg.graphics.beginGradientFill("radial", [0xDDD0AA, 0xC6B689], [1, 1], [1, 255], gradMat);
			bg.graphics.drawRect(0, 0, 1920, 998 /*1280,718*/);
			bg.graphics.endFill();
			boardBitmapData.draw(bg);
			
			//We clear out the undo buffer with a copy of just a blank background:
			undoStack = new Vector.<BitmapData>;
			var undoBuffer:BitmapData = new BitmapData(boardWidth, boardHeight, false);
			undoBuffer.copyPixels(boardBitmapData, undoBuffer.rect, new Point(0, 0));
			undoStack.push(undoBuffer);
		}
		
		private function undo(evt:MouseEvent):void
		{
			if (undoStack.length > 1)
			{
				boardBitmapData.copyPixels(undoStack[undoStack.length - 2], boardBitmapData.rect, new Point(0, 0));
				undoStack.splice(undoStack.length - 1, 1);
			}
			tipLayer.graphics.clear();
			
			LogInteractiveActivityEvent("The last brush stroke was undone", false)
		}
		
		//this function assists with creating colors for the gradients.
		private function darkenColor(c:uint, factor:Number):uint
		{
			var r:Number = (c >> 16);
			var g:Number = (c >> 8) & 0xFF;
			var b:Number = c & 0xFF;
			
			var newRed:Number = Math.min(255, r * factor);
			var newGreen:Number = Math.min(255, g * factor);
			var newBlue:Number = Math.min(255, b * factor);
			
			return (newRed << 16) | (newGreen << 8) | (newBlue);
		}
		
		private function LogInteractiveActivityEvent(description:String, isGameHasExpired:Boolean = false):void
		{
			// comment the following line to test
			ExternalInterface.call("FlashCall", description, isGameHasExpired);
		}
		
		private function timedFunctionGame():void
		{
			LogInteractiveActivityEvent("Game timeout has expired", true);
		}
		
		private function GetColourDescriptor(swatch:Object):String
		{
			if (swatch.red1 == 0 && swatch.green1 == 0 && swatch.blue1 == 178 && swatch.red2 == 0 && swatch.green2 == 0 && swatch.blue2 == 255)
				return "Blue";
			else if (swatch.red1 == 107 && swatch.green1 == 71 && swatch.blue1 == 35 && swatch.red2 == 153 && swatch.green2 == 102 && swatch.blue2 == 51)
				return "Brown";
			else if (swatch.red1 == 0 && swatch.green1 == 178 && swatch.blue1 == 178 && swatch.red2 == 0 && swatch.green2 == 255 && swatch.blue2 == 255)
				return "Cyan";
			else if (swatch.red1 == 35 && swatch.green1 == 142 && swatch.blue1 == 0 && swatch.red2 == 51 && swatch.green2 == 204 && swatch.blue2 == 0)
				return "Green";
			else if (swatch.red1 == 178 && swatch.green1 == 0 && swatch.blue1 == 178 && swatch.red2 == 255 && swatch.green2 == 0 && swatch.blue2 == 255)
				return "Pink";
			else if (swatch.red1 == 178 && swatch.green1 == 88 && swatch.blue1 == 0 && swatch.red2 == 255 && swatch.green2 == 127 && swatch.blue2 == 0)
				return "Orange";
			else if (swatch.red1 == 88 && swatch.green1 == 0 && swatch.blue1 == 88 && swatch.red2 == 127 && swatch.green2 == 0 && swatch.blue2 == 127)
				return "Purple";
			else if (swatch.red1 == 178 && swatch.green1 == 0 && swatch.blue1 == 0 && swatch.red2 == 255 && swatch.green2 == 0 && swatch.blue2 == 0)
				return "Red";
			else if (swatch.red1 == 178 && swatch.green1 == 178 && swatch.blue1 == 0 && swatch.red2 == 255 && swatch.green2 == 255 && swatch.blue2 == 0)
				return "Yellow";
			else if (swatch.red1 == 0 && swatch.green1 == 0 && swatch.blue1 == 0 && swatch.red2 == 0 && swatch.green2 == 0 && swatch.blue2 == 0)
				return "Black";
			else if (swatch.red1 == 59 && swatch.green1 == 59 && swatch.blue1 == 59 && swatch.red2 == 85 && swatch.green2 == 85 && swatch.blue2 == 85)
				return "Dark Grey";
			else if (swatch.red1 == 107 && swatch.green1 == 107 && swatch.blue1 == 107 && swatch.red2 == 153 && swatch.green2 == 153 && swatch.blue2 == 153)
				return "Light Grey";
			else if (swatch.red1 == 229 && swatch.green1 == 229 && swatch.blue1 == 229 && swatch.red2 == 255 && swatch.green2 == 255 && swatch.blue2 == 255)
				return "White";
			
			return "Unknown";
		}
	}
}