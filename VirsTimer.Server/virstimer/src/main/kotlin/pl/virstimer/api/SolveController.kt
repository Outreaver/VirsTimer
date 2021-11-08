package pl.virstimer.api

import org.springframework.http.HttpStatus
import org.springframework.http.ResponseEntity
import org.springframework.security.access.annotation.Secured
import org.springframework.security.core.Authentication
import org.springframework.web.bind.annotation.*
import pl.virstimer.model.Solve
import pl.virstimer.model.SolveChange
import pl.virstimer.model.Solved
import pl.virstimer.repository.SolveCustomRepository

import pl.virstimer.repository.SolveRepository
import java.util.*


@RestController
@RequestMapping("/solve")
class SolveController(
    val repository: SolveRepository,
    val customRepository: SolveCustomRepository) {

    @GetMapping("/all")
    fun findAllSolves(authentication: Authentication): List<Solve> {

        return repository.findAllByUserId(authentication.name)
    }

    @GetMapping("/{objectId}")
    @Secured("ROLE_USER")
    fun getSolve(@PathVariable id: String, authentication: Authentication): Solve = repository.findOneByIdAndUserId(id, authentication.name)

    @GetMapping("/user") // TODO: Change
    @Secured("ROLE_USER")
    fun findAllUser(authentication: Authentication): List<Solve> {

        return repository.findAllByUserId(authentication.name)
    }

    @GetMapping("/session/{sessionId}")
    @Secured("ROLE_USER")
    fun findAllSession(@PathVariable sessionId: String, authentication: Authentication): List<Solve> = repository.findAllBySessionIdAndUserId(sessionId, authentication.name)


    @PostMapping
    @Secured("ROLE_USER")
    fun createSolve(@RequestBody request: SolveRequest, authentication: Authentication): ResponseEntity<Solve> {
        val solve = repository.save(
            Solve(
                id = UUID.randomUUID().toString(),
                userId = authentication.name,
                sessionId = request.sessionId,
                scramble = request.scramble,
                time = request.time,
                timestamp = request.timestamp,
                solved = request.solved
            )
        )
        return ResponseEntity(solve, HttpStatus.CREATED)
    }


    @PatchMapping("/{solveId}")
    @Secured("ROLE_USER")
    fun updateSolve(@PathVariable solveId: String, @RequestBody solveChange: SolveChange, authentication: Authentication): ResponseEntity<Unit> {
        customRepository.updateSolve(solveId, solveChange, authentication.name)
        return ResponseEntity.ok(Unit)
    }

    @DeleteMapping("delete/{solveId}")
    @Secured("ROLE_USER")
    fun deleteSolve(@PathVariable solveId: String, authentication: Authentication) = repository.deleteSolveByIdAndUserId(solveId, authentication.name)

    @DeleteMapping("delete/all")
    @Secured("ROLE_USER")
    fun deleteSolves(authentication: Authentication){
        repository.deleteSolveByUserId(authentication.name)
    }

}

data class SolveRequest(
    val sessionId: String,
    val scramble: String,
    val time: Long,
    val timestamp: Long,
    val solved: Solved
)
