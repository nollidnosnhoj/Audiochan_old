import React from "react";
import { ChakraProvider } from "@chakra-ui/react";
import { AppProps as NextAppProps } from "next/app";
import dynamic from "next/dynamic";
import { QueryClient, QueryClientProvider } from "react-query";
import { Hydrate } from "react-query/hydration";
import PageLoader from "~/components/PageLoader";
import { UserProvider } from "~/contexts/UserContext";
import AudioPlayerProvider from "~/components/providers/AudioPlayerProvider";
import theme from "~/lib/theme";
import queryClient from "~/lib/queryClient";
import { CurrentUser } from "~/features/user/types";
import AuthProvider from "~/components/providers/AuthProvider";

interface AppProps extends NextAppProps {
  user?: CurrentUser;
}

const AudioPlayer = dynamic(() => import("~/components/AudioPlayer"), {
  ssr: false,
});

function App({ Component, user, pageProps }: AppProps) {
  const queryClientRef = React.useRef<QueryClient>();
  if (!queryClientRef.current) {
    queryClientRef.current = queryClient;
  }

  return (
    <>
      <QueryClientProvider client={queryClientRef.current}>
        <Hydrate state={pageProps.dehydratedState}>
          <ChakraProvider resetCSS theme={theme}>
            <UserProvider initialUser={user || null}>
              <AuthProvider>
                <AudioPlayerProvider>
                  <PageLoader color={theme.colors.primary[500]} />
                  <Component {...pageProps} />
                  <AudioPlayer />
                </AudioPlayerProvider>
              </AuthProvider>
            </UserProvider>
          </ChakraProvider>
        </Hydrate>
      </QueryClientProvider>
    </>
  );
}

export default App;
