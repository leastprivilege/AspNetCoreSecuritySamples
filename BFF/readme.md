This sample shows a possible approach for securing a SPA using

* a server-side backend for user authentication and session management
* SameSite cookies
* automatic token management
* proxying calls to back-end services

No explicit anti-forgery protection has been implemented, because we assume SameSite cookies work for you.

See [this](https://leastprivilege.com/2019/01/18/an-alternative-way-to-secure-spas-with-asp-net-core-openid-connect-oauth-2-0-and-proxykit/) blog post for more details.
