# Samples for various ASP.NET Core Security Features

Many samples you find on the internet try to demo a certain feature in the context of a larger use-case.
I find this often distracting.

The samples here follow the "write the simplest sample to show the feature" philosophy. Maybe you find this more useful.

### Local Authentication
Simple local login page with cookie-based session.

### External Authentication with callback
Google authentication will simple callback logic on an MVC controller.

### OidcTokensApis
Sample showing cookie-based session with OpenID Connect-based authentication and automated access token lifetime management.

### Authorization
Some variations of policy- and resource-based authorization

### DataProtection
Shows simple protect/unprotect calls and corresponding key container.

### SPA & BFF
Two variations of how to protect a JavaScript application. SPA is pure JS where the token management happens in the front-end,
whereas BFF is a combination of UI front-end and back-end token management.

### Device Flow
Simple device flow client.
