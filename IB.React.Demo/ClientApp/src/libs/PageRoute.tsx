import React, {useMemo} from "react";
import {Route, Switch} from "react-router";
import {Home, HomeInfo, IRoutedPage, PageList, TestPage, TestPageInfo} from "../pages";
import AuthRoute from "./AuthRoute";
import {Login, LoginInfo} from "../pages/auth";
import {IUser} from "./apis/Interfaces";

interface IPageRouteProps {
    currentUser: IUser | null
}

export default function PageRoute({
    currentUser,
}: IPageRouteProps) {
    // 모든 라우트 정보를 조회합니다.
    const routes = useMemo<IRoutedPage[]>(() => {
        return PageList;
    }, [])

    return (
        <Switch>
            <Route path="/" exact component={Home} />
            <Route path={HomeInfo.path} exact component={Home} />
            <Route path={LoginInfo.path} component={Login} />
            <AuthRoute currentUser={currentUser} roles={["Admin", "Member"]}
                       path={TestPageInfo.path} component={TestPage}
            />


            {/*{*/}
            {/*    // 모든 라우트 정보를 Route로 만듭니다.*/}
            {/*    routes.map(route => <Route key={route.pageId} {...route} />)*/}
            {/*}*/}
        </Switch>
    )
}