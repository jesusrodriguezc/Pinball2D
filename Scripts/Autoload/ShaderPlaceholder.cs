using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinball.Components {
	public partial class ShaderPlaceholder: Node {
		private ShaderMaterial shader;
		public CompressedTexture2D Palette { 
			get { return (CompressedTexture2D) shader.GetShaderParameter("palette"); } 
			set { shader.SetShaderParameter("palette", value); } 
		}
		public override void _Ready () {
			var settings = GetNode<SettingsManager>("/root/SettingsManager");
			shader = GetNode<ColorRect>("CanvasLayer/ColorRect")?.Material as ShaderMaterial;

			Palette = (CompressedTexture2D)GD.Load(settings.settingsData.GetPaletteURL());


		}
	}
}
