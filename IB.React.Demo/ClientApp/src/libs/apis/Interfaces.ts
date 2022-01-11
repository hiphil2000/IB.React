export interface IMenu {
    menuId: string;
    parentId: string;
    menuName: string;
    menuPath: string;
    displayNo: number;
    menuYn: boolean;
    pageYn: boolean;
    useYn: boolean;
    visibleYn: boolean;
}

export interface IUser {
    userNo: number;
    userId: string;
    userName: string;
    role: string;
}

export interface IJwtPayload {
    issuer: number,
    expirationTime: string,
    issuedAt: string,
    jwtId: string,
    subject: string,
    role: string
}

export interface ICommonResponse<TData> {
    success: boolean;
    data: TData | null;
    message: string | null;
}