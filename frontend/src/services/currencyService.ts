import { HttpService } from "./common/httpService";
import { BASE_URL } from "@/configurations/HttpConfiguration";
import type CurrencyModel from "@/models/currencyModel";

export default class CurrencyService extends HttpService {
    constructor() {
        super(BASE_URL + "/currency");
    };

    public async getList(): Promise<Array<CurrencyModel>> {
        const response = await this.get(`/list`);

        if (response.isOk()) {
            return response.value.result as Array<CurrencyModel>;
        }
        throw response.error;
    }

}