import React from "react";
import {IRoutedPage} from "../Interfaces";
import PageTemplate from "../../components/layout/PageTemplate";
import LoginForm from "../../containers/auth/LoginForm";

/**
 * 로그인 페이지 입니다.
 * @constructor
 */
export default function Login() {
    return (
        <PageTemplate>
            <LoginForm />
        </PageTemplate>
    )
}

/**
 * 로그인 페이지 라우트 정보입니다.
 */
export const LoginInfo: IRoutedPage = {
    pageId: "LOGIN",
    pageName: "로그인",
    path: "/auth/login",
    component: Login
}