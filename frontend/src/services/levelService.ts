import type LevelModel from "@/models/levelModel";
import { HttpService } from "./common/httpService";
import { BASE_URL } from "@/configurations/HttpConfiguration";
import { LevelTypeEnum } from "@/models/levelModel";
import type UserModel from "@/models/userModel";

export default class LevelService extends HttpService {
    constructor() {
        super(BASE_URL + "/boost");
    };

    public async getLevels(): Promise<Array<LevelModel>> {
        const getLevels = localStorage.getItem('levels');
        if (getLevels) {
            try {
                return JSON.parse(getLevels) as Array<LevelModel>;
            } catch (exception) { }
        }
        const response = await this.get(`/levels`);

        if (response.isOk()) {
            const levels: Array<LevelModel> = response.value.result;

            localStorage.setItem('levels', JSON.stringify(levels));

            return levels;
        }
        throw response.error;
    }
    public async levelUp(levelType: LevelTypeEnum): Promise<UserModel> {
        const response = await this.post(`/levelUp/${levelType.toString()}`);

        if (response.isOk()) {
            const user: UserModel = response.value.result;

            localStorage.setItem('user', JSON.stringify(user));

            return user;
        }

        throw response.error;
    }
}