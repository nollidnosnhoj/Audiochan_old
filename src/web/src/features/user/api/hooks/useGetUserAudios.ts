import { useCallback } from "react";
import { QueryKey } from "react-query";
import { AudioView } from "~/features/audio/api/types";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { OffsetPagedList } from "~/lib/types";

type UseGetUserAudiosParams = {
  size?: number;
};

export const GET_USER_AUDIOS_QUERY_KEY = (username: string): QueryKey => [
  "userAudios",
  username,
];

export function useGetUserAudios(
  username: string,
  params: UseGetUserAudiosParams = {},
  options: UseInfinitePaginationOptions<AudioView> = {}
): UseInfinitePaginationReturnType<AudioView> {
  const fetcher = useCallback(
    async (offset: number) => {
      const { data } = await request<OffsetPagedList<AudioView>>({
        method: "get",
        url: `users/${username}/audios`,
        params: {
          ...params,
          offset,
        },
      });
      return data;
    },
    [username, params]
  );

  return useInfinitePagination(GET_USER_AUDIOS_QUERY_KEY(username), fetcher, {
    ...options,
    enabled: !!username && (options.enabled ?? true),
  });
}
