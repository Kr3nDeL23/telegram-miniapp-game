import type BaseModel from "./baseModel";

export default interface TaskModel extends BaseModel {
    path: string;
    challengeId: string;
    title: string;
    isCompleted: boolean;
}
