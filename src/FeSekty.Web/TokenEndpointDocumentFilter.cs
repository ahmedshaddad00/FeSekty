using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FeSekty.Web;

/// <summary>
/// /connect/token is served by the OpenIddict middleware (not an MVC controller), so Swagger
/// does not discover it. This filter documents it so the mobile team can see and try it.
/// </summary>
public class TokenEndpointDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "grant_type", "client_id" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["grant_type"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "password = login, refresh_token = renew tokens",
                    Enum = new List<JsonNode> { "password", "refresh_token" }
                },
                ["client_id"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Always FeSekty_Mobile (public client, no secret)",
                    Default = JsonValue.Create("FeSekty_Mobile")
                },
                ["username"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "User name or email (grant_type=password)"
                },
                ["password"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "grant_type=password"
                },
                ["scope"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Space separated. offline_access is required to get a refresh token (grant_type=password)",
                    Default = JsonValue.Create("openid profile email roles offline_access FeSekty")
                },
                ["refresh_token"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "grant_type=refresh_token"
                }
            }
        };

        var operation = new OpenApiOperation
        {
            Summary = "Mobile login / refresh token (OpenIddict)",
            Description = "Login: grant_type=password + username + password + scope. " +
                          "Refresh: grant_type=refresh_token + refresh_token. " +
                          "Send as application/x-www-form-urlencoded (not JSON). " +
                          "Returns access_token, refresh_token, id_token, expires_in. " +
                          "Use the access_token as: Authorization: Bearer <access_token>.",
            RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/x-www-form-urlencoded"] = new OpenApiMediaType { Schema = schema }
                }
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse { Description = "access_token, token_type, expires_in, id_token, refresh_token" },
                ["400"] = new OpenApiResponse { Description = "OAuth error, e.g. invalid_grant (wrong username/password or bad refresh token)" }
            }
        };

        swaggerDoc.Paths ??= new OpenApiPaths();
        swaggerDoc.Paths["/connect/token"] = new OpenApiPathItem
        {
            Operations = new Dictionary<HttpMethod, OpenApiOperation> { [HttpMethod.Post] = operation }
        };
    }
}
