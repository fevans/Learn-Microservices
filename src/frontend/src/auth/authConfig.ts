import {WebStorageStateStore} from 'oidc-client-ts';

export const oidcConfig = {
    authority:              'https://localhost:5005',
    client_id:              'play-frontend',
    redirect_uri:           'http://localhost:3000/authentication/login-callback',
    post_logout_redirect_uri: 'http://localhost:3000/authentication/logout-callback',
    response_type:          'code',
    scope:                  'openid profile roles catalog.fullaccess inventory.fullaccess',
    automaticSilentRenew:   true,
    userStore: new WebStorageStateStore({ store: window.localStorage }),
};