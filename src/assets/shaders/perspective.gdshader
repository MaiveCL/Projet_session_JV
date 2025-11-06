shader_type canvas_item;

uniform float perspective_factor : hint_range(0,1) = 0.5;
uniform bool enable_perspective = true;

void fragment() {
    vec2 uv = UV;

    if (enable_perspective) {
        // Appliquer la perspective
        float scale = 1.0 - uv.y * perspective_factor;
        float x_centered = uv.x - 0.5;
        x_centered *= scale;

        x_centered = mod(x_centered + 0.5, 1.0) - 0.5;

        uv.x = x_centered + 0.5;
    }

    COLOR = texture(TEXTURE, uv);
}
