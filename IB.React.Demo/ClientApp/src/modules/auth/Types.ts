import * as Actions from "./Actions";
import {ActionType} from "typesafe-actions";
import {IUser} from "../../libs/apis/Interfaces";
import {AsyncState} from "../ReducerUtils";
import {AxiosError} from "axios";

export type AuthAction = ActionType<typeof Actions>;

export type AuthState = {
    user: IUser | null,
    getCurrentUser: AsyncState<IUser, AxiosError>,
}
