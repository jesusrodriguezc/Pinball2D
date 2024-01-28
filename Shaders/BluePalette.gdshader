shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE :hint_screen_texture, filter_linear_mipmap;

uniform sampler2D palette;
uniform sampler2D luminosityPalette;
uniform float minValue;
uniform float maxValue;
uniform float lightThreshold;


float grayScale(vec3 color)
{
    return (color.r + color.g + color.b) / 3.0;
}
float applyRangeChange(float grayScaleValue)
{
	if (grayScaleValue < minValue) grayScaleValue = minValue;
	else if (grayScaleValue >= maxValue) grayScaleValue = maxValue;
	
	//return grayScaleValue;
	// 0.8f * 0.5f + 0.25f = 0.4f + 0.25f = 0.65f
	return (maxValue-minValue) * grayScaleValue + minValue;
}

float humanGrayScale (vec3 color)
{
	// Y = .2126 * R + .7152 * G + .0722 * B
	float grayScaleValue = color.r * .2126 + color.g * .7152 + color.b * .0722;
	grayScaleValue = applyRangeChange(grayScaleValue);
	return grayScaleValue;
}


void fragment() {
	
    int palette_size = textureSize(palette, 0).x;
    float palette_pixel_size = 1.0 / float(palette_size);

	COLOR = texture(SCREEN_TEXTURE, SCREEN_UV);
	float screenGrayColor = humanGrayScale(COLOR.rgb);
	
	// 0.0 - 1.0 * 8 -> 
	float colorStepsToDo = floor(screenGrayColor * float(palette_size)) + 0.5;
    vec3 palette_col = texture(palette, vec2(colorStepsToDo * palette_pixel_size, 0.0)).rgb;

    COLOR.rgb = palette_col;
	//COLOR.rgb = vec3(screenGrayColor, screenGrayColor, screenGrayColor);

	
}

