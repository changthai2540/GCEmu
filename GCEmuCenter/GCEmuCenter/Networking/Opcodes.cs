//-----------------------------------------------------------------------
// GCEmu - A Grand Chase Season 4 Eternal Emulator
// Copyright © 2017 Roverde
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//-----------------------------------------------------------------------

namespace GCEmuCenter
{
    public enum CenterOpcodes
    {
        HEART_BIT_NOT,
        SET_SECURITY_KEY_NOT,
        ENU_VERIFY_ACCOUNT_REQ,
        ENU_VERIFY_ACCOUNT_ACK,
        ENU_SERVER_LIST_NOT,
        ENU_WAIT_TIME_NOT,
        EVENT_CLOSE_CONNECTION_NOT,
        ENU_CONNECT_TO_SERVER_REQ,
        ENU_CONNECT_TO_SERVER_ACK,
        ENU_CHANNEL_NEWS_NOT,
        ENU_ITEM_BUY_INFO_NOT,
        ENU_LOG_OUT_NOT,
        ENU_CLIENT_SCRIPT_INFO_NOT,
        ENU_CLIENT_CONTENTS_OPEN_NOT,
        ENU_NEW_CLIENT_CONTENTS_OPEN_NOT,
        ENU_SOCKET_TABLE_INFO_NOT,
        ENU_OVERLAP_FILE_INFO,
        ENU_GUIDE_BOOK_LIST_REQ,
        ENU_GUIDE_BOOK_LIST_ACK,
        ENU_TEXTURE_DYNAMIC_LOAD_REQ,
        ENU_TEXTURE_DYNAMIC_LOAD_ACK,
        ENU_LOADING_IMAGE_REQ,
        ENU_LOADING_IMAGE_ACK,
        ENU_CLIENT_CONTENTS_FIRST_INIT_INFO_REQ,
        ENU_CLIENT_CONTENTS_FIRST_INIT_INFO_ACK,
        ENU_CLIENT_PING_CONFIG_REQ,
        ENU_CLIENT_PING_CONFIG_ACK,
        ENU_SHAFILENAME_LIST_REQ,
        ENU_SHAFILENAME_LIST_ACK,
        ENU_STAY_SERVER_TIMEOUT,
        ENU_CASHBACK_RATIO_INFO_NOT
    };
}
