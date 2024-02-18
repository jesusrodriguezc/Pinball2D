using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class MapGenerator : StaticBody2D {
	[Export] public CollisionPolygon2D[] collisionPolygons;
	[Export] public CollisionShape2D[] collisionShapes;
	[Export] public Godot.Color paintingColor;
	[Export] public bool CollisionEnabled;
	private uint storedCollisionLayer;

	public override void _Ready () {

		if (collisionPolygons == null || collisionPolygons.Length == 0) {
			collisionPolygons = GetChildren().OfType<CollisionPolygon2D>().ToArray();
		}
		if (collisionShapes == null || collisionShapes.Length == 0) {
			collisionShapes = GetChildren().OfType<CollisionShape2D>().ToArray();

		}

		storedCollisionLayer = CollisionLayer;

		QueueRedraw();
	}

	public override void _PhysicsProcess (double delta) {
		QueueRedraw();
	}

	public void EnableCollision (bool enable) {
		if (CollisionEnabled == enable) {
			return;
		}

		CollisionEnabled = enable;

		if (CollisionEnabled) {
			CollisionLayer = storedCollisionLayer;
		}
		else {
			storedCollisionLayer = CollisionLayer;
			CollisionLayer = 0;
		}
		
	}
	public override void _Draw () {

		if (collisionPolygons != null) {
			foreach(var collisionPolygon in collisionPolygons) {
				DrawSetTransform(collisionPolygon.Position, collisionPolygon.Rotation, collisionPolygon.Scale);
				DrawPolygon(collisionPolygon.Polygon, Enumerable.Repeat(paintingColor, collisionPolygon.Polygon.Length).ToArray());
			}
		}

		if (collisionShapes != null) {
			foreach (var collisionShape in collisionShapes) {
				DrawCollisionShape(collisionShape, paintingColor);
			}
		}
	}

	private void DrawCollisionShape (CollisionShape2D collisionShape, Godot.Color color) {

		if (collisionShape == null) return;

		DrawSetTransform(collisionShape.Position, collisionShape.Rotation, collisionShape.Scale);

		switch (collisionShape.Shape) {
			case CircleShape2D circle:
				DrawCircle(Vector2.Zero, circle.Radius, color);
				break;

			case RectangleShape2D rect:
				//var rectangle = new Rect2(rect.GetRect().Position, rect.GetRect().Size);
				DrawRect(rect.GetRect(), color);
				break;

			case CapsuleShape2D caps:
				var rotation = Rotation + collisionShape.Rotation;
				var distanceBetweenCenterAndCircleCenter = new Vector2(0f, (caps.Height / 2f) - caps.Radius);
				var centerTopCircle = - distanceBetweenCenterAndCircleCenter;
				var centerDownCircle = distanceBetweenCenterAndCircleCenter;

				var rectangleHeight = caps.Height - caps.Radius * 2f;
				var rectangleTopLeftPoint = - (new Vector2(caps.Radius, rectangleHeight / 2));

				DrawCircle(centerTopCircle, caps.Radius, color);
				DrawCircle(centerDownCircle, caps.Radius, color);

				var rectangleBetweenCircles = new Rect2(rectangleTopLeftPoint, caps.Radius * 2f, rectangleHeight);
				DrawRect(rectangleBetweenCircles, color);

				break;
			default:
				GD.PrintErr($"Tipo de forma no compatible {collisionShape.Shape.GetType().Name}");
				break;
		}
	}

}

