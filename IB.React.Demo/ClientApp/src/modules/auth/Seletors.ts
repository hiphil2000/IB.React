import {IRootState} from "../index";

export const userSelector = (state: IRootState) => state.auth.user; 
export const currentUserSelector = (state: IRootState) => state.auth.getCurrentUser;