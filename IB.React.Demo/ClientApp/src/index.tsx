import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import {createBrowserHistory} from "history";
import configureStore from "./modules/ConfigureStore";
import {Provider} from "react-redux";
import {ConnectedRouter} from "connected-react-router";
import loadUser from "./libs/UserLoader";

// connected-react-router 사용을 위한 History 생성
// base 태그의 href (%PUBLIC_URL%)을 사용합니다.
const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href") as string;
const history = createBrowserHistory();

// 스토어를 생성합니다.
const store = configureStore(history);

(async () => {
    // 사용자 정보를 조회하고 화면을 랜더링합니다.
    await loadUser(store);
    ReactDOM.render(
        <React.StrictMode>
            <Provider store={store}>
                <ConnectedRouter history={history}>
                    <App/>
                </ConnectedRouter>
            </Provider>
        </React.StrictMode>,
        document.getElementById('root')
    );
})();

