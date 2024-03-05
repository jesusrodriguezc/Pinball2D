﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinball.Components {
	public partial class ShaderPlaceholder: Node {
		private ColorRect shaderContainer;
		private ShaderMaterial shader;
		public CompressedTexture2D Palette { 
			get { return (CompressedTexture2D) shader.GetShaderParameter("palette"); } 
			set { shader.SetShaderParameter("palette", value); } 
		}
		public override void _Ready () {
			var settings = GetNode<SettingsManager>("/root/SettingsManager");
			shaderContainer = GetNode<ColorRect>("CanvasLayer/ColorRect") as ColorRect;
			//shaderContainer.Visible = false;
			shader = shaderContainer?.Material as ShaderMaterial;

			var paletteUrl = settings.settingsData.GetPaletteURL();
			if (!string.IsNullOrWhiteSpace(paletteUrl)) {
				Palette = (CompressedTexture2D)GD.Load(paletteUrl);
			}


		}

		public void Apply () {
			shaderContainer.Visible = true;

		}
	}
}
