import type BaseModel from "./baseModel";

export default interface ChallengeModel extends BaseModel {
    name: string;
    bonus: number;
    image: string;
    isCompleted: boolean;
}
