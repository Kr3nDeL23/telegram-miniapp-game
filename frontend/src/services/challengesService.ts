import { HttpService } from "./common/httpService";
import { BASE_URL } from "@/configurations/HttpConfiguration";
import type ChallengeModel from "@/models/challengeModel";
import type TaskModel from "@/models/taskModel";

export interface CompletTaskResponse {
    path: string;
    isCompleted: boolean;
}

export default class ChallengeService extends HttpService {
    constructor() {
        super(BASE_URL + "/challenge");
    };

    public async completTask(id: string): Promise<CompletTaskResponse> {
        const response = await this.post(`/completTask/` + id);
        if (response.isOk()) {
            return response.value.result as CompletTaskResponse;
        }
        throw response.error;
    }
    public async completChallenge(id: string): Promise<ChallengeModel> {
        const response = await this.post(`/completChallenge/` + id);
        if (response.isOk()) {
            const userModel: UserModel = response.value.result.user;

            localStorage.setItem('user', JSON.stringify(userModel));

            return response.value.result.challenge as ChallengeModel;
        }
        throw response.error;
    }
    public async getList(): Promise<Array<ChallengeModel>> {
        const response = await this.get(`/list`);

        if (response.isOk()) {
            return response.value.result as Array<ChallengeModel>;
        }
        throw response.error;
    }
    public async getTasks(id: string): Promise<Array<TaskModel>> {
        const response = await this.get(`/getTasks/` + id);

        if (response.isOk()) {
            return response.value.result as Array<TaskModel>;
        }
        throw response.error;
    }

}