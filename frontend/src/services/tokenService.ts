import { HttpService } from "./common/httpService";
import { BASE_URL } from "@/configurations/HttpConfiguration";
import type LeagueModel from "@/models/leagueModel";
import type LevelModel from "@/models/levelModel";
import type TokenModel from "@/models/tokenModel";
import type UserModel from "@/models/userModel";

export default class TokenService extends HttpService {
    constructor() {
        super(BASE_URL + "/token");
    };

    public async signIn(webAppData: string): Promise<TokenModel> {
        const response = await this.post('/signin', { webAppData });

        if (response.isOk()) {
            const tokenModel: TokenModel = response.value.result.token;

            localStorage.setItem('token', JSON.stringify(tokenModel));

            const leagues: Array<LeagueModel> = response.value.result.leagues;

            localStorage.setItem('leagues', JSON.stringify(leagues));

            const levels: Array<LevelModel> = response.value.result.levels;

            localStorage.setItem('levels', JSON.stringify(levels));

            const user: UserModel = response.value.result.user;

            localStorage.setItem('user', JSON.stringify(user));

            return tokenModel;
        }
        throw response.error;
    }


}