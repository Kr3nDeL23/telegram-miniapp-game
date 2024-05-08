import { HttpService } from "./common/httpService";
import { BASE_URL } from "@/configurations/HttpConfiguration";
import type SkinModel from "@/models/skinModel";
import type UserModel from "@/models/userModel";

export default class SkinService extends HttpService {
    constructor() {
        super(BASE_URL + "/skin");
    };

    public async getList(size: number = 50, page: number = 1): Promise<Array<SkinModel>> {
        const response = await this.get(`/list?size=${size}&page=${page}`);

        if (response.isOk()) {
            return response.value.result as Array<SkinModel>;
        }
        throw response.error;
    }

    public async set(id: string): Promise<UserModel> {
        const response = await this.post(`/set/${id}`, {});

        if (response.isOk()) {
            const userModel: UserModel = response.value.result;

            localStorage.setItem('user', JSON.stringify(userModel));

            return userModel;
        }
        throw response.error;
    }
    public async buy(id: string): Promise<UserModel> {
        const response = await this.post(`/buy/${id}`, {});

        if (response.isOk()) {
            const userModel: UserModel = response.value.result;

            localStorage.setItem('user', JSON.stringify(userModel));

            return userModel;
        }

        throw response.error;
    }


}