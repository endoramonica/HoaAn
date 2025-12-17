/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { UserStatus } from './UserStatus';
export type UserDetailDTO = {
    id?: string;
    email?: string | null;
    name?: string | null;
    phone?: string | null;
    avatarUrl?: string | null;
    isActive?: boolean;
    status?: UserStatus;
    statusText?: string | null;
    lastLogin?: string | null;
    storeId?: string;
    storeName?: string | null;
    createdAt?: string;
    roles?: Array<string> | null;
};

