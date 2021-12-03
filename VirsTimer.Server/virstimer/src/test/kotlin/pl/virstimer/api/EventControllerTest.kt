package pl.virstimer.api

import org.junit.jupiter.api.Test

import org.junit.jupiter.api.BeforeEach
import org.junit.jupiter.api.Disabled
import org.junit.jupiter.api.extension.ExtendWith
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc
import org.springframework.boot.test.context.SpringBootTest
import org.springframework.data.mongodb.core.query.Criteria
import org.springframework.data.mongodb.core.query.Query
import org.springframework.test.annotation.DirtiesContext
import org.springframework.test.context.ActiveProfiles
import org.springframework.test.context.junit.jupiter.SpringExtension
import org.springframework.test.web.servlet.result.MockMvcResultMatchers
import pl.virstimer.TestCommons
import pl.virstimer.model.Event

@SpringBootTest
@ExtendWith(SpringExtension::class)
@AutoConfigureMockMvc
@ActiveProfiles("test")
//@DirtiesContext(classMode = DirtiesContext.ClassMode.AFTER_EACH_TEST_METHOD)
class EventControllerTest : TestCommons() {

    @BeforeEach
    fun injections(){ before_each() }

    @Test
    fun posting_event_ok()
    {
        val loginDetails = registerAndLogin("user-1", "user-1-pass")
        createEvent("1", "FIVE_BY_FIVE", loginDetails.authHeader)
            .andExpect(MockMvcResultMatchers.status().isCreated)


        val foundItems = mongoTemplate.find(
                Query.query(Criteria.where("userId").`is`("user-1")),
                Event::class.java
            )

        foundItems.size == 1
        with(foundItems.get(0)) {
            this.userId == "user-1"
            this.puzzleType == "FIVE_BY_FIVE"
        }
    }

    @Test
    @Disabled // TODO: Fix, broken due to VIRSTIMER-58
    fun should_patch_event()
    {
        val loginDetails = registerAndLogin("user-1", "user-1-pass")

        createEvent("1", "FIVE_BY_FIVE", loginDetails.authHeader).andExpect(MockMvcResultMatchers.status().isCreated)
        val event  = mongoTemplate.find(Query(Criteria.where("userId").`is`("user-1")), Event::class.java).first()
        assert(event.puzzleType == "FIVE_BY_FIVE")

        patchEvent("updatedPuzzle", loginDetails.authHeader,event.id).andExpect(MockMvcResultMatchers.status().isOk)
        assert(mongoTemplate.find(Query(Criteria.where("userId").`is`("user-1")), Event::class.java).first().puzzleType == "updatedPuzzle")
    }

    @Test
    fun should_not_allow_posting_event_if_not_logged_in() {
        createEvent(
            "1",
            "FIVE_BY_FIVE ",
            "not-existing-token"
        ).andExpect(MockMvcResultMatchers.status().is4xxClientError)
    }

}