import * as React from "react";
import {RouteComponentProps} from "react-router";

export interface IPage {
    pageId: string;
    parentId?: string;
    pageName: string;
    pageNameEn?: string;
    orderNo?: number;
    pageYn?: boolean;
    menuYn?: boolean;
    useYn?: boolean;
}

export interface IRouteData {
    path: string;
    component: React.ComponentType<RouteComponentProps<any>> | React.ComponentType<any> | undefined;
    exact?: boolean;
}

export interface IRoutedPage extends IPage, IRouteData {
    
}