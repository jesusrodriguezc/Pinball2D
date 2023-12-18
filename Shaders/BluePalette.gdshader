shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE :hint_screen_texture, filter_linear_mipmap;

uniform sampler2D palette;
uniform float minValue;
uniform float maxValue;


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
	
	vec3 source_col = texture(SCREEN_TEXTURE, SCREEN_UV).rgb;
    vec3 closest_col = vec3(0.0);
    float smallest_error = .1f;

    int palette_size = textureSize(palette, 0).x;
    float palette_pixel_size = 1.0 / float(palette_size);
	
	COLOR = texture(SCREEN_TEXTURE, SCREEN_UV);
	float screenGrayColor = humanGrayScale(COLOR.rgb);
	
	float stepValue = 1.f/float(palette_size);
	
	// 0.3f -> 0.3f ^* 6f = 
	float stepsToDo = floor((abs(1.f -screenGrayColor) * float(palette_size)));
    vec3 palette_col = texture(palette, vec2(stepsToDo * palette_pixel_size, 0.0)).rgb;
    float paletteGrayColor = humanGrayScale(palette_col);
	
//    for (int x = 0; x < palette_size; x++)
//    {
//        vec3 palette_col = texture(palette, vec2(float(x) * palette_pixel_size, 0.0)).rgb;
//        float paletteGrayColor = humanGrayScale(palette_col);
//		float error = distance(screenGrayColor, paletteGrayColor);
//        if (error < smallest_error)
//        {
//            closest_col = palette_col;
//            smallest_error = error;
//        }
//    }

    COLOR.rgb = palette_col;
	
}

