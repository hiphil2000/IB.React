import React from "react";
import {IRoutedPage} from "../Interfaces";
import HomeContainer from "../../containers/home/HomeContainer";
import PageTemplate from "../../components/layout/PageTemplate";

/**
 * 홈 페이지입니다.
 * @constructor
 */
export default function Home() {
    return (
        <PageTemplate>
            <HomeContainer />
        </PageTemplate>
    )
}

/**
 * 홈 페이지 라우트 정보입니다.
 */
export const HomeInfo: IRoutedPage = {
    pageId: "HOME",
    pageName: "홈",
    path: "/",
    exact: true,
    component: Home
};