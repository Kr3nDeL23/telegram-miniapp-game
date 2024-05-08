import { HttpService } from "./common/httpService";
import { BASE_URL } from "@/configurations/HttpConfiguration";

import type UserModel from "@/models/userModel";
import type SquadModel from "@/models/squadModel";
import type LeagueModel from "@/models/leagueModel";

export default class LeagueService extends HttpService {
    constructor() {
        super(BASE_URL + "/league");
    };

    public async getList(): Promise<Array<LeagueModel>> {
        const getLeagues = localStorage.getItem('leagues');
        if (getLeagues) {
            try {
                return JSON.parse(getLeagues) as Array<LeagueModel>;
            } catch (exception) { }
        }


        const response = await this.get(`/list`);

        if (response.isOk()) {
            const leagues: Array<LeagueModel> = response.value.result;

            localStorage.setItem('leagues', JSON.stringify(leagues));

            return leagues;
        }
        throw response.error;
    }
    public async getSquads(id: string, query: string = "", size: number = 50, page: number = 1): Promise<Array<SquadModel>> {
        const response = await this.get(`/squads/${id}/?query=${query}&size=${size}&page=${page}`);

        if (response.isOk()) {
            return response.value.result as Array<SquadModel>;
        }
        throw response.error;
    }
 
    public async getMembers(id: string, query: string = "", size: number = 50, page: number = 1): Promise<Array<UserModel>> {
        const response = await this.get(`/members/${id}/?query=${query}&size=${size}&page=${page}`);

        if (response.isOk()) {
            return response.value.result as Array<UserModel>;
        }
        throw response.error;
    }


}