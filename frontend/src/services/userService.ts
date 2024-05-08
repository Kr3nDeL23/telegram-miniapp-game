import { HttpService } from "./common/httpService";
import { BASE_URL } from "@/configurations/HttpConfiguration";
import type PaginationModel from "@/models/paginationModel";
import type UserModel from "@/models/userModel";

export default class UserService extends HttpService {
    constructor() {
        super(BASE_URL + "/user");
    };
    public getUserFromStorage(): UserModel | undefined {
        const getJsonString = localStorage.getItem('user');

        if (getJsonString) {
            try {
                const user: UserModel = JSON.parse(getJsonString);

                const getSeconds = Math.ceil(
                    (new Date().getTime() - new Date(user.lastClickDate).getTime()) / 1000);

                user.availableEnergy = Math.min(
                    user.availableEnergy + getSeconds * user.rechargeEnergyLevel.value, user.limitEnergyLevel.value);

                return user;
            } catch (exception) { console.log(exception) }
        }
    };
    public async transfer(count: number, id: number): Promise<UserModel> {
        const response = await this.post(`/transfer`, { userId: id, count });

        if (response.isOk()) {
            const userModel: UserModel = response.value.result;

            localStorage.setItem('user', JSON.stringify(userModel));

            return userModel;
        }
        throw response.error;
    }
    public async profile(): Promise<UserModel> {

        const getUserStorage = this.getUserFromStorage();
        console.log(getUserStorage)
        if (getUserStorage) return getUserStorage;

        const response = await this.get('');

        if (response.isOk()) {
            const userModel: UserModel = response.value.result;

            localStorage.setItem('user', JSON.stringify(userModel));

            return userModel;
        }
        throw response.error;
    }
    public async getMembers(query: string = "", size: number = 50, page: number = 1): Promise<PaginationModel<Array<UserModel>>> {
        const response = await this.get(`/members?query=${query}&size=${size}&page=${page}`);

        if (response.isOk()) {
            return response.value as PaginationModel<Array<UserModel>>;
        }
        throw response.error;
    }


}