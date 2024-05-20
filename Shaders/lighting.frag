#version 330 core
//In this tutorial it might seem like a lot is going on, but really we just combine the last tutorials, 3 pieces of source code into one
//and added 3 extra point lights.
struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_specular1;
    float     shininess;
    float     triplanarStrength;
};
//This is the directional light struct, where we only need the directions
struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform DirLight dirLight;
//This is our pointlight where we need the position aswell as the constants defining the attenuation of the light.
struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
//We have a total of 4 point lights now, so we define a preprossecor directive to tell the gpu the size of our point light array
#define NR_POINT_LIGHTS 1
uniform PointLight pointLights[NR_POINT_LIGHTS];
//This is our spotlight where we need the position, attenuation along with the cutoff and the outer cutoff. Plus the direction of the light
struct SpotLight{
    vec3  position;
    vec3  direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};
uniform SpotLight spotLight;

uniform Material material;
uniform vec3 viewPos;
uniform vec3 localPos;

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

//Here we have some function prototypes, these are the signatures the gpu will use to know how the
//parameters of each light calculation is layed out.
//We have one function per light, since this makes it so we dont have to take up to much space in the main function.
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

vec4 GetTriplanarMapping(sampler2D tex, vec3 wPos, vec3 wNorm, float scale)
{
    vec3 n = abs(wNorm);
    vec4 res;

    if (scale != 0)
    {
        vec4 colX = texture(tex, wPos.yz * scale);
        vec4 colY = texture(tex, wPos.zx * scale);
        vec4 colZ = texture(tex, wPos.xy * scale);

        vec3 blend = normalize(max(n, 0.0001));
        blend /= (blend.x + blend.y + blend.z);

        res = blend.x * colX + blend.y * colY + blend.z * colZ;
    }
    else
    {
        res = texture(tex, TexCoords);
    }

    return res;
}

void main()
{
    //properties
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    //phase 1: Directional lighting
    vec3 result = CalcDirLight(dirLight, norm, viewDir);
    //phase 2: Point lights
    for(int i = 0; i < NR_POINT_LIGHTS; i++)
        result += CalcPointLight(pointLights[i], norm, viewDir, viewDir);
    //phase 3: Spot light
    result += CalcSpotLight(spotLight, norm, FragPos, viewDir);    

    //triplanar
    /*vec4 map = GetTriplanarMapping(FragPos, norm, material.triplanarStrength);
    result *= vec3(map);*/

    FragColor = vec4(result, 1.0);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);

    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    //combine results
    vec4 mapDiffuse = GetTriplanarMapping(material.texture_diffuse1, FragPos, normal, material.triplanarStrength);
    vec4 mapSpecular = GetTriplanarMapping(material.texture_specular1, FragPos, normal, material.triplanarStrength);

    vec3 ambient  = light.ambient * vec3(mapDiffuse);
    vec3 diffuse  = light.diffuse  * diff * vec3(mapDiffuse);
    vec3 specular = light.specular * spec * vec3(mapSpecular);

    return (ambient + diffuse + specular);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);

    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    //attenuation
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));

    //combine results
    vec4 mapDiffuse = GetTriplanarMapping(material.texture_diffuse1, FragPos, normal, material.triplanarStrength);
    vec4 mapSpecular = GetTriplanarMapping(material.texture_specular1, FragPos, normal, material.triplanarStrength);

    vec3 ambient  = light.ambient * vec3(mapDiffuse);
    vec3 diffuse  = light.diffuse  * diff * vec3(mapDiffuse);
    vec3 specular = light.specular * spec * vec3(mapSpecular);

    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;

    return (ambient + diffuse + specular);
} 
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{

    //diffuse shading
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(normal, lightDir), 0.0);

    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    //attenuation
    float distance    = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));

    //spotlight intensity
    float theta     = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    //combine results
    vec4 mapDiffuse = GetTriplanarMapping(material.texture_diffuse1, FragPos, normal, material.triplanarStrength);
    vec4 mapSpecular = GetTriplanarMapping(material.texture_specular1, FragPos, normal, material.triplanarStrength);

    vec3 ambient  = light.ambient * vec3(mapDiffuse);
    vec3 diffuse  = light.diffuse  * diff * vec3(mapDiffuse);
    vec3 specular = light.specular * spec * vec3(mapSpecular);

    ambient  *= attenuation;
    diffuse  *= attenuation * intensity;
    specular *= attenuation * intensity;

    return (ambient + diffuse + specular);
}