import axios, { type AxiosRequestConfig } from 'axios';
import { merge } from 'lodash-es';
import { err, ok, Result } from 'neverthrow';

import { HttpException } from './httpException';
import type TokenModel from '@/models/tokenModel';

export class HttpService {
    protected urlBase = '/';

    public constructor(path?: string) {
        this.urlBase = path ?? '/';
    }

    protected getConfig(): AxiosRequestConfig {
        const token = this.getToken()?.accessToken;

        return {
            headers: {
                Authorization: token ? `Bearer ${token}` : 'false'
            },
        };
    }
    getToken = (): TokenModel | null => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                return JSON.parse(token) as TokenModel;
            } catch (error) {}
        }
        return null;
    };
    clearStorage = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        localStorage.removeItem('leagues');
        localStorage.removeItem('levels');
    };
    isAuthenticated = (): boolean => {
        if (this.getToken()) return true;

        return false;
    };
    protected async get<T>(path = '', configOverrides: AxiosRequestConfig | undefined = undefined,): Promise<Result<T, HttpException>> {
        return await this.requestResultWrapper<T>(path, configOverrides, (fullPath, config) => {
            return axios.get(fullPath, config);
        });
    }

    protected async post<T>(path = '', data: unknown = undefined, configOverrides: AxiosRequestConfig | undefined = undefined,): Promise<Result<T, HttpException>> {
        return await this.requestResultWrapper<T>(path, configOverrides, (fullPath, config) => {
            return axios.post(fullPath, data, config);
        });
    }

    protected async put<T>(path = '', data: unknown = undefined, configOverrides: AxiosRequestConfig | undefined = undefined,): Promise<Result<T, HttpException>> {
        return await this.requestResultWrapper<T>(path, configOverrides, (fullPath, config) => {
            return axios.put(fullPath, data, config);
        });
    }

    protected async patch<T>(path = '', data: unknown = undefined, configOverrides: AxiosRequestConfig | undefined = undefined,): Promise<Result<T, HttpException>> {
        return await this.requestResultWrapper<T>(path, configOverrides, (fullPath, config) => {
            return axios.patch(fullPath, data, config);
        });
    }

    protected async delete<T>(path = '', configOverrides: AxiosRequestConfig | undefined = undefined,): Promise<Result<T, HttpException>> {
        return await this.requestResultWrapper<T>(path, configOverrides, (fullPath, config) => {
            return axios.delete(fullPath, config);
        });
    }

    private async requestResultWrapper<T>(subPath: string, configOverrides: AxiosRequestConfig | undefined, request: (fullPath: string, config: AxiosRequestConfig | undefined) => Promise<{ data: unknown } | null>,): Promise<Result<T, HttpException>> {
        if (subPath.length > 0 && subPath[0] !== '/') subPath = `/${subPath}`;
        const config = merge(this.getConfig() || {}, configOverrides || {});
        try {
            const responseData: T = (await request(`${this.urlBase}${subPath}`, config))?.data as T;
            return ok(responseData);
        } catch (e) {
            const except = err(new HttpException(e));

            if (except.error.responseStatus == 401) {
                this.clearStorage();
                window.location.reload();
            }
            return except;
        }
    }
}
