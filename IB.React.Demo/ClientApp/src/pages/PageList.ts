import {IRoutedPage} from "./Interfaces";
import {HomeInfo, TestPageInfo} from "./home";
import {LoginInfo} from "./auth";

const pageList: IRoutedPage[] = [
    HomeInfo,
    TestPageInfo,
    LoginInfo
]

export default pageList;