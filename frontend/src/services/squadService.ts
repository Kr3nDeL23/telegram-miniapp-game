import type SquadModel from "@/models/squadModel";
import { HttpService } from "./common/httpService";
import { BASE_URL } from "@/configurations/HttpConfiguration";
import type UserModel from "@/models/userModel";
import type PaginationModel from "@/models/paginationModel";

export default class SquadService extends HttpService {
    constructor() {
        super(BASE_URL + "/squad");
    };
    public async Join(id: string): Promise<UserModel> {
        const response = await this.post(`/join/${id}`);

        if (response.isOk()) {
            const user: UserModel = response.value.result;

            localStorage.setItem('user', JSON.stringify(user));

            return user;
        }
        throw response.error;
    }
    public async Left(): Promise<UserModel> {
        const response = await this.post(`/left`);

        if (response.isOk()) {
            const user: UserModel = response.value.result;

            localStorage.setItem('user', JSON.stringify(user));

            return user;
        }
        throw response.error;
    }

    public async getSquad(id: string): Promise<SquadModel> {
        const response = await this.get(`/get/${id}`);

        if (response.isOk()) {
            return response.value.result as SquadModel;
        }
        throw response.error;
    }

    public async getList(query: string, size: number = 50, page: number = 1): Promise<PaginationModel<Array<SquadModel>>> {
        const response = await this.get(`/search?query=${query}&size=${size}&page=${page}`);

        if (response.isOk()) {
            return response.value as PaginationModel<Array<SquadModel>>;
        }
        throw response.error;
    }

    public async getMembers(id: string, query: string = "", size: number = 50, page: number = 1): Promise<PaginationModel<Array<UserModel>>> {
        const response = await this.get(`/members/${id}/?query=${query}&size=${size}&page=${page}`);

        if (response.isOk()) {
            return response.value as PaginationModel<Array<UserModel>>;
        }
        throw response.error;
    }


}