package pl.virstimer.api

import org.junit.jupiter.api.BeforeEach
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.extension.ExtendWith
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc
import org.springframework.boot.test.context.SpringBootTest
import org.springframework.test.context.junit.jupiter.SpringExtension
import org.springframework.test.web.servlet.request.MockMvcRequestBuilders
import org.springframework.test.web.servlet.result.MockMvcResultMatchers
import pl.virstimer.TestCommons
import pl.virstimer.model.Solve
import pl.virstimer.model.Solved

@SpringBootTest
@ExtendWith(SpringExtension::class)
@AutoConfigureMockMvc
class SolveControllerTest : TestCommons() {
    @BeforeEach
    fun injections() {
        before_each()
        mongoTemplate.insert(Solve(null, "user-1", "session_name1", "scramble", 1L, 1L, Solved.OK))
        mongoTemplate.insert(Solve(null, "user-2", "session_name2", "scramble", 1L, 1L, Solved.OK))
    }

    @Test
    fun should_return_solves() {
        val loginDetails = registerAndLogin("user-1", "user-1-pass")

        mockMvc.perform(
            MockMvcRequestBuilders.get("/solves/all")
                .authorizedWith(loginDetails.authHeader)
        )
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].id").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].userId").value("user-1"))
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].sessionId").value("session_name1"))
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].scramble").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].time").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].timestamp").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].solved").isNotEmpty)
            .andExpect(MockMvcResultMatchers.status().isOk)
    }

    @Test
    fun should_return_solve_for_user() {
        val loginDetails = registerAndLogin("user-1", "user-1-pass")

        mockMvc.perform(
            MockMvcRequestBuilders.get("/solves/user")
                .authorizedWith(loginDetails.authHeader)
        )
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].id").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].userId").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].sessionId").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].scramble").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].time").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].timestamp").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].solved").isNotEmpty)
            .andExpect(MockMvcResultMatchers.status().isOk)
    }

    @Test
    fun should_return_solve_for_session() {
        val loginDetails = registerAndLogin("user-1", "user-1-pass")

        mockMvc.perform(
            MockMvcRequestBuilders.get("/solves/session/session_name1")
                .authorizedWith(loginDetails.authHeader)
        )
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].id").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].userId").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].sessionId").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].scramble").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].time").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].timestamp").isNotEmpty)
            .andExpect(MockMvcResultMatchers.jsonPath("$[0].solved").isNotEmpty)
            .andExpect(MockMvcResultMatchers.status().isOk)

        // Should return empty when accessing other user session

        mockMvc.perform(
            MockMvcRequestBuilders.get("/solves/session/session_name2")
                .authorizedWith(loginDetails.authHeader)
        )
            .andExpect(MockMvcResultMatchers.jsonPath("$").isEmpty)

    }
}