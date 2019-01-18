This sample shows a possible approach securing a SPA using

* a server-side backend for user authentication and session management
* SameSite cookies
* automatic token management
* proxying calls to back-end services

No explicit anti-forgery protection has been implemented, because we rely on SameSite cookies.