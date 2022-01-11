import React, {useEffect} from "react";
import {Redirect, Route, RouteComponentProps, useHistory} from "react-router";
import useAuth from "./hooks/UseAuth";
import {Typography} from "@mui/material";
import {LoginInfo} from "../pages/auth";
import {IUser} from "./apis/Interfaces";

interface IAuthRouteProps {
    currentUser: IUser | null;
    roles: string[];
    path: string;
    component: React.ComponentType<RouteComponentProps<any>> | React.ComponentType<any> | undefined;
    exact?: boolean;
    redirectTo?: string;
}

/**
 * 인증 여부를 판단하여 라우팅을 진행합니다.
 * 인증되어있지 않다면 @redirectTo 로 리다이렉트합니다.
 * 
 * @param authenticated 인증 여부
 * @param redirectTo 리다이렉트 할 주소
 * @param path 실제 라우팅 주소
 * @param component 라우팅 대상 컴포넌트
 * @param exact 주소 정확 여부 (react-router)
 * @constructor
 */
export default function AuthRoute({
    currentUser,
    roles,
    path,
    component,
    exact,
    redirectTo
}: IAuthRouteProps) {
    return (
        currentUser && roles.includes(currentUser.role) ?
            <Route path={path} component={component} exact={exact} /> :
            <Redirect to={redirectTo || LoginInfo.path} />
    )
}