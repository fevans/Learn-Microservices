import { User } from 'oidc-client-ts';

export interface PlayEconomyUser extends User {
    profile: User['profile'] & {
        role?:      string | string[];
        gil?:       string;
        gil_spent?: string;
    };
}